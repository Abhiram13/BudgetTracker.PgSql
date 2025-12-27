using System.Net;
using BudgetTracker.Gateway.Middlewares;
using Abhiram.Extensions.DotEnv;
using BudgetTracker.Gateway.Security;
using BudgetTracker.Shared.Utilities;
using Abhiram.Abstractions.Logging;
using BudgetTracker.Gateway.Models;
using BudgetTracker.Gateway.Interfaces;
using Microsoft.Extensions.Options;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Gateway.Services;
using Abhiram.Secrets.Providers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog(template: "[{Level:u3}] [Source: {SourceContext}] {Message:lj}{NewLine}{Exception}");
builder.Logging.AddFilter("Yarp.ReverseProxy.Forwarder.HttpForwarder", LogLevel.Warning);
builder.Configuration.Sources.Add(new GatewayAppSecretsSource(new SecretManagerService()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddOptions<GatewayAppSecrets>().Bind(builder.Configuration.GetSection("Api")).ValidateOnStart();
builder.Services.AddOptions<GatewayAppSecrets>().Bind(builder.Configuration.GetSection("Yarp")).ValidateOnStart();
builder.Services.AddSingleton<IGatewayAppSecrets>(sp => sp.GetRequiredService<IOptions<GatewayAppSecrets>>().Value);
builder.Services.AddSingleton<IYarpApiKeyAppSecret>(sp => sp.GetRequiredService<IOptions<GatewayAppSecrets>>().Value);
builder.Services.AddAuthentication().AddScheme<ApiKeySchemaOptions, ApiKeyHandler>(ApiKeySchemaOptions.DefaultSchema, _ => {});
builder.Services.AddScoped<TraceIdProvider>();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy().RequireAuthorization();
app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<BadGatewayMiddleware>();
app.UseMiddleware<TraceProviderMiddleware>();
app.Run();
