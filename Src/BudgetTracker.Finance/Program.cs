using System.Net;
using Microsoft.EntityFrameworkCore;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Extensions;
using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

string baseDir = AppContext.BaseDirectory;
string environment = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile(Path.Combine(baseDir, "sharedsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(baseDir, $"sharedsettings.{environment}.json"), optional: false, reloadOnChange: true);

builder.AddConsoleGoogleSeriLog();
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddOptions<AppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddCollections();
builder.Services.AddSwaggerGen();
builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3001";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("DB Migration is starting...");
        
        PostgresSecrets secrets = scope.ServiceProvider.GetRequiredService<AppSecrets>().Postgres;
        DbContextOptionsBuilder<WriteDbContext> contextOptionsBuilder = new DbContextOptionsBuilder<WriteDbContext>();
        string connectionString = $"Host={secrets.Host};Port={secrets.MigratePort};Database={secrets.Database};Username={secrets.MigrateUsername};Password={secrets.MigratePassword}";
        contextOptionsBuilder.UseNpgsql(connectionString);

        using (WriteDbContext writeDbContext = new WriteDbContext(contextOptionsBuilder.Options))
        {
            writeDbContext.Database.Migrate();
        }
        
        logger.LogInformation("DB Migration completed");
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "Exception at Finance Server DB Migrate Setup - {ErrorMessage}", e.InnerException?.Message ?? e.Message);
    }
}

app.UseApplicationServices();
app.Run();

namespace BudgetTracker.Finance
{
    public partial class Program { }
}