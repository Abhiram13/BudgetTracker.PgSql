using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.Extensions.Options;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Gateway.Middlewares;
using BudgetTracker.Gateway.Security;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Gateway.Models;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Extensions;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Security;
using BudgetTracker.Gateway.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Transforms;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

string baseDir = AppContext.BaseDirectory;
string environment = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile(Path.Combine(baseDir, "sharedsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(baseDir, $"sharedsettings.{environment}.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine("/secrets/", "gateway-secrets.json"), optional: true, reloadOnChange: true);

builder.AddConsoleGoogleSeriLog();
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.LoadJwtConfiguration(builder.Configuration);
builder.Services.AddOptions<GatewayAppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSingleton<GatewayAppSecrets>(option => option.GetRequiredService<IOptions<GatewayAppSecrets>>().Value);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transform =>
    {
        if (builder.Environment.IsDevelopment())
        {
            transform.ConfigJwtAuthentication();
        }

        if (builder.Environment.IsEnvironment(SharedConstants.Environments.GOOGLECLOUD))
        {
            transform.ConfigGoogleOAuth();
        }
    });
builder.Services.AddAuthentication().AddScheme<ApiKeySchemaOptions, ApiKeyHandler>(ApiKeySchemaOptions.DefaultSchema, _ => {});
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy => policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
});

builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3000";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    const string URL = "/api-docs/openapi/{documentName}.json";
    app.MapScalarApiReference("/api-docs", options =>
    {
        options.Title = "Budget Tracker Finance documentation";
        options.Servers = new List<ScalarServer>
        {
            new ScalarServer("http://localhost:3000", "Development Server")
        };
        options
            .WithOpenApiRoutePattern(URL)
            .AddPreferredSecuritySchemes("ApiKey")
            .AddApiKeyAuthentication("ApiKey", apiKey =>
            {
                apiKey.Name = "API_KEY";                
            });
    })
    .AllowAnonymous();
}

app.UseMiddleware<TraceProviderMiddleware>();
app.UseMiddleware<BadGatewayMiddleware>();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => new ApiResponse { StatusCode = HttpStatusCode.OK, Message = "This is YARP API Gateway" });
app.Run();
