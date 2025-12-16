using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Microsoft.EntityFrameworkCore;
using System.Net;
using BudgetTracker.Dues;
using BudgetTracker.Dues.Interfaces;
using BudgetTracker.Dues.Repository;
using BudgetTracker.Dues.Services;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Middlwares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

// builder.AddConsoleGoogleSeriLog(template: "[{Level:u3}] [TraceId: {trace_id}] [Source: {SourceContext}] {Message:lj}{NewLine}{Exception}");
builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = YarpApiKeySchemaOptions.DefaultSchema;
    options.DefaultChallengeScheme = YarpApiKeySchemaOptions.DefaultSchema;
}).AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});
builder.Services.AddScoped<IDueRepository, DueRepository>();
builder.Services.AddScoped<DueService>();
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddDbContext<WriteDBContext>(async (provider, options) =>
{
    string? postgresHost = "localhost";
    string? postgresPort = "5432";
    string? postgresDatabase = "BudgetTracker.Due";
    string? postgresUsername = "postgres";
    string? postgresPassword = "postgres";
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
    try
    {
        WriteDBContext context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        context.Database.Migrate();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
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
