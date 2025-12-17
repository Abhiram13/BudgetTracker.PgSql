using Microsoft.EntityFrameworkCore;
using System.Net;
using BudgetTracker.Dues;
using BudgetTracker.Dues.Interfaces;
using BudgetTracker.Dues.Repository;
using BudgetTracker.Dues.Services;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Middlwares;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Dues.Models;
using Abhiram.Secrets.Providers.Interface;
using Abhiram.Secrets.Providers;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog(template: "[{Level:u3}] [Source: {SourceContext}] {Message:lj}{NewLine}{Exception}");
builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDueRepository, DueRepository>();
builder.Services.AddScoped<DueService>();
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddSingleton<ISecretManager, SecretManagerService>();
builder.Services.AddSingleton<IYarpApiKeyAppSecret, DueAppSecrets>();
builder.Services.AddSingleton<IDueAppSecrets, DueAppSecrets>();
builder.Services.AddHostedService<SecretHostService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = YarpApiKeySchemaOptions.DefaultSchema;
    options.DefaultChallengeScheme = YarpApiKeySchemaOptions.DefaultSchema;
}).AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});
builder.Services.AddDbContext<WriteDBContext>(async (provider, options) =>
{
    IDueAppSecrets appSecrets = provider.GetRequiredService<IDueAppSecrets>();

    string? postgresHost = appSecrets.PostgresHost;
    string? postgresPort = appSecrets.PostgresPort;
    string? postgresDatabase = appSecrets.PostgresDatabase;
    string? postgresUsername = appSecrets.PostgresUsername;
    string? postgresPassword = appSecrets.PostgresPassword;
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
        IDueAppSecrets appSecrets = scope.ServiceProvider.GetRequiredService<IDueAppSecrets>();
        if (!string.IsNullOrEmpty(appSecrets.PostgresHost))
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