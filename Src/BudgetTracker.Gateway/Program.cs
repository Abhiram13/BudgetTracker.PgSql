using System.Net;
using Microsoft.Extensions.Options;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Gateway.Middlewares;
using BudgetTracker.Gateway.Security;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Gateway.Models;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using BudgetTracker.Warehouse.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog();
builder.Logging.AddFilter("Yarp.ReverseProxy.Forwarder.HttpForwarder", LogLevel.Warning);
builder.Environment.EnvironmentName = "Development";
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddOptions<GatewayAppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSingleton<GatewayAppSecrets>(option => option.GetRequiredService<IOptions<GatewayAppSecrets>>().Value);
builder.Services.AddSingleton<YarpApiKeySecret>(opt => opt.GetRequiredService<IOptions<GatewayAppSecrets>>().Value.Secrets);
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<ActivityLoggerMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy().RequireAuthorization();
app.UseHttpsRedirection();
app.UseMiddleware<BadGatewayMiddleware>();
app.Run();
