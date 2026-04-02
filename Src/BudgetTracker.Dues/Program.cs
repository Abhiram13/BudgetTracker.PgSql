using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Dues;
using BudgetTracker.Dues.Interfaces;
using BudgetTracker.Dues.Repository;
using BudgetTracker.Dues.Services;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Middlwares;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Dues.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog();
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddOptions<DueAppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DueAppSecrets>(sp => sp.GetRequiredService<IOptions<DueAppSecrets>>().Value);
builder.Services.AddScoped<IDueRepository, DueRepository>();
builder.Services.AddScoped<DueService>();
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = YarpApiKeySchemaOptions.DefaultSchema;
    options.DefaultChallengeScheme = YarpApiKeySchemaOptions.DefaultSchema;
}).AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});
builder.Services.AddDbContext<WriteDBContext>((provider, options) =>
{
    PostgresSecrets appSecrets = provider.GetRequiredService<DueAppSecrets>().Postgres;

    string? postgresHost = appSecrets.Host;
    string? postgresPort = appSecrets.Port;
    string? postgresDatabase = appSecrets.Database;
    string? postgresUsername = appSecrets.Username;
    string? postgresPassword = appSecrets.Password;
    string connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
    options.UseNpgsql(connectionString);
});
builder.WebHost.ConfigureKestrel((_, server) =>
{
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3002";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

using (IServiceScope? scope = app.Services.CreateScope())
{
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        DueAppSecrets appSecrets = scope.ServiceProvider.GetRequiredService<DueAppSecrets>();
        
        if (!string.IsNullOrEmpty(appSecrets.Postgres.Host))
        {
            WriteDBContext context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
            context.Database.Migrate();
        }        
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "Exception at Due Server DB Migrate Setup ({0})", e.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.UseMiddleware<ValidateTraceIdMiddleware>();
app.Run();

public partial class Program { }