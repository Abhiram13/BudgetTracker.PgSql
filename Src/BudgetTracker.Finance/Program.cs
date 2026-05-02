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
// using Google.Apis.Auth.OAuth2;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry;
using OpenTelemetry.Exporter.OpenTelemetryProtocol;
// using Google.Cloud.Diagnostics.AspNetCore3;
// using Google.Cloud.Diagnostics.Common;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog();
builder.Environment.EnvironmentName = "Development";
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddOptions<AppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddCollections();
builder.Services.AddSwaggerGen();

#region commented code

// builder.Services.AddGoogleTraceForAspNetCore(new AspNetCoreTraceOptions
// {
//     ServiceOptions = new TraceServiceOptions()
//     {
//         ProjectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID"),
//         Options = TraceOptions.Create(qpsSampleRate: 1.0)
//     }
// });
// builder.Services.AddGoogleTrace(new TraceServiceOptions
// {
//     ProjectId = "",
//     Options = TraceOptions.Create(qpsSampleRate: 1.0)
// });
// builder.Logging.AddOpenTelemetry(options =>
// {
//     options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("budget-tracker-service")).AddConsoleExporter();
// });
// builder.Logging.AddFilter("OpenTelemetry", LogLevel.Error);
// builder.Logging.AddFilter("System.Net.Http", LogLevel.Warning);
// builder.Services.AddOpenTelemetry()
//     // .ConfigureResource(resource =>
//     // {
//     //     resource.AddService("budget-tracker-service").AddAttributes(new Dictionary<string, object> { ["deployment.environment"] = "development" });
//     // })
//     .WithTracing(tracing =>
//     {
//         tracing
//             .SetSampler(new AlwaysOnSampler())
//             // .AddConsoleExporter()
//             .AddAspNetCoreInstrumentation()
//             .AddHttpClientInstrumentation()
//             .AddSource("TransactionService")
//             .AddOtlpExporter(o =>
//             {
//                 // 1. Use the full secure endpoint
//                 o.Endpoint = new Uri("https://otlp-gateway-prod-ap-south-1.grafana.net/otlp");
//                 // o.Endpoint = new Uri("https://telemetry.googleapis.com/v1/traces");
//                 // 2. Identify the project
//                 // string projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID") ?? "";
//                 
//                 // o.Headers = $"x-goog-user-project={project-id}";
//                 o.Headers = "Authorization=Basic ";
//                 // 3. Ensure we use gRPC for this specific endpoint
//                 // o.Protocol = OtlpExportProtocol.HttpProtobuf;
//                 o.Protocol = OtlpExportProtocol.HttpProtobuf;
//                 o.ExportProcessorType = ExportProcessorType.Simple;
//             });
//     });
// builder.Services.AddOpenTelemetry()
//     .ConfigureResource(resource => resource.AddService("budget-tracker-service"))
//     .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation().AddConsoleExporter())
//     .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation().AddConsoleExporter());

// builder.Services.AddOpenTelemetry()
//     .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter(options => options.Endpoint = new Uri("https://telemetry.googleapis.com")));

// async Task<string> GetAccessToken()
// {
//     GoogleCredential? credential = await GoogleCredential.GetApplicationDefaultAsync();
//     string url = "https://www.googleapis.com/auth/cloud-platform";
//     var token = await credential?.GetA
//
//     return token;
// }
AppContext.SetSwitch("OpenTelemetrySdkEventSource", true);

builder.Logging.AddFilter("OpenTelemetry", LogLevel.Trace);
builder.Services.AddOpenTelemetry()
    .WithTracing(trace =>
    {
        trace
            .ConfigureResource(resource => resource
                    .AddService("budget-tracker-finance") // Give it a clear name!
            )
            // .AddSource("test-span")
            // .SetSampler(new AlwaysOnSampler())
            .AddAspNetCoreInstrumentation()
            // .AddConsoleExporter()
            // .AddHttpClientInstrumentation()
            .AddHttpClientInstrumentation(options =>
            {
                // This is the CRITICAL part:
                // Don't trace calls to Google APIs (Logging, Trace, etc.)
                options.FilterHttpRequestMessage = (request) => !request.RequestUri.Host.Contains("googleapis.com");
            })
            .AddOtlpExporter(o =>
            {
                // o.Endpoint = new Uri("https://telemetry.googleapis.com/v1/traces");
                o.Endpoint = new Uri("http://localhost:9999");
                o.Protocol = OtlpExportProtocol.HttpProtobuf;
                // o.ExportProcessorType = ExportProcessorType.Simple;
                // o.HttpClientFactory = () =>
                // {
                //     HttpClient client = new HttpClient();
                //
                //     client.DefaultRequestHeaders.TryAddWithoutValidation(
                //         "X-Debug-Mode", "true");
                //
                //     return client;
                // };
            });
        // .AddOtlpExporter(options =>
        // {
        //     options.Endpoint = new Uri("https://telemetry.googleapis.com/v1/traces");
        //     options.Protocol = OtlpExportProtocol.HttpProtobuf;
        //
        //     // IMPORTANT: don't hardcode token in real apps
        //     // options.Headers = $"Authorization=Bearer {GetAccessToken()}";
        // });
    });

#endregion

builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3001";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

TracerProvider traceProvider = Sdk.CreateTracerProviderBuilder().AddOtlpExporter().Build();

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