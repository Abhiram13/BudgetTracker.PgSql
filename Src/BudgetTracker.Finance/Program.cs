using System.Net;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Extensions;
using Microsoft.EntityFrameworkCore;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Interfaces;
using Abhiram.Secrets.Providers;
using BudgetTracker.Finance.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog(template: "[{Level:u3}] [Source: {SourceContext}] {Message:lj}{NewLine}{Exception}");
builder.Configuration.Sources.Add(new FinanceAppSecretsSource(new SecretManagerService()));
builder.Services.AddOptions<AppSecrets>().Bind(builder.Configuration.GetSection("Postgres")).ValidateOnStart();
builder.Services.AddOptions<AppSecrets>().Bind(builder.Configuration.GetSection("Yarp")).ValidateOnStart();
builder.Services.AddCollections();
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
        WriteDbContext context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        context.Database.Migrate();
        logger.LogInformation("DB Migration completed");
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "Exception at Finance Server DB Migrate Setup ({0})", e.Message);
    }
}

app.UseApplicationServices();
app.Run();

namespace BudgetTracker.Finance
{
    public partial class Program { }
}