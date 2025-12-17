using System.Net;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Extensions;
using Microsoft.EntityFrameworkCore;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog(template: "[{Level:u3}] [Source: {SourceContext}] {Message:lj}{NewLine}{Exception}");
builder.Services.AddHostedService<SecretHostService>();
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
        IFinanceAppSecrets appSecrets = scope.ServiceProvider.GetRequiredService<IFinanceAppSecrets>();
        if (!string.IsNullOrEmpty(appSecrets.PostgresHost))
        {
            WriteDbContext context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            context.Database.Migrate();
        }        
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "Exception at Finance Server DB Migrate Setup ({0})", e.Message);
    }
}

app.UseApplicationServices();
app.Run();

public partial class Program { }