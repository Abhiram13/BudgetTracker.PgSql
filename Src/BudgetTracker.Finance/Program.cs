using System.Net;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Finance.Configurations;
using BudgetTracker.Finance.Extensions;
using BudgetTracker.Shared.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

string baseDir = AppContext.BaseDirectory;
string environment = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile(Path.Combine(baseDir, "sharedsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(baseDir, $"sharedsettings.{environment}.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine("/secrets/", "finance-secrets.json"), optional: true, reloadOnChange: true); // NOTE: secrets to get loaded from GCP secret manager

builder.AddConsoleGoogleSeriLog();
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddCollections(builder.Configuration);
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OpenApiTransformer>();
    options.AddSchemaTransformer((schema, _, _) =>
    {
        schema.Example = null;
        schema.Default = null;
        return Task.CompletedTask;
    });
});
builder.WebHost.ConfigureKestrel((context, server) =>
{
    AppSecrets? secrets = context.Configuration.Get<AppSecrets>();
    int port = secrets?.ServerPort ?? 3001;
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/api-docs/openapi/{documentName}.json");
}

app.UseApplicationServices();
app.MapGet("/", () => new ApiResponse { StatusCode = HttpStatusCode.OK, Message = "This is Downstream Finance API services" });
app.Run();

namespace BudgetTracker.Finance
{
    public partial class Program { }
}