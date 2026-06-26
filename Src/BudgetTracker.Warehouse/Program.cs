using System.Net;
using Microsoft.Extensions.Options;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Shared.Extensions;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Middlwares;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Warehouse.Models;
using BudgetTracker.Warehouse.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

string baseDir = AppContext.BaseDirectory;
string environment = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile(Path.Combine(baseDir, "sharedsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(baseDir, $"sharedsettings.{environment}.json"), optional: false, reloadOnChange: true);

builder.AddConsoleGoogleSeriLog();
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddOptions<WarehouseAppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSingleton<WarehouseAppSecrets>(sp => sp.GetRequiredService<IOptions<WarehouseAppSecrets>>().Value);
builder.Services.AddSingleton<BigQueryService>();
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddScoped<SubscriberService>();
// builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddHostedService<SubscriberBackgroundService>();

builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3004";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.Run();

namespace BudgetTracker.Warehouse
{
    public partial class Program { }
}
