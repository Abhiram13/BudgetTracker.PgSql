using System.Net;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Providers;
using Abhiram.Secrets.Providers.Interface;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Middlwares;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Warehouse.Interfaces;
using BudgetTracker.Warehouse.Models;
using BudgetTracker.Warehouse.Services;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog(template: "[{Level:u3}] [Source: {SourceContext}] {Message:lj}{NewLine}{Exception}");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOptions<WarehouseAppSecrets>().Bind(builder.Configuration.GetSection("Config")).ValidateOnStart();
builder.Services.AddOptions<WarehouseAppSecrets>().Bind(builder.Configuration.GetSection("Yarp")).ValidateOnStart();
builder.Services.AddSingleton<ISecretManager, SecretManagerService>();
builder.Services.AddSingleton<IYarpApiKeyAppSecret>(sp => sp.GetRequiredService<IOptions<WarehouseAppSecrets>>().Value);
builder.Services.AddSingleton<IWarehouseAppSecrets>(sp => sp.GetRequiredService<IOptions<WarehouseAppSecrets>>().Value);
builder.Configuration.Sources.Add(new WarehouseAppSecretsSource(new SecretManagerService()));
builder.Services.AddSingleton<BigQueryService>();
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddScoped<SubscriberService>();
builder.Services.AddHostedService<SecretHostService>();
builder.Services.AddHostedService<SubscriberBackgroundService>(); // TODO: This step blocking application shutdown
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = YarpApiKeySchemaOptions.DefaultSchema;
    options.DefaultChallengeScheme = YarpApiKeySchemaOptions.DefaultSchema;
}).AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});

builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3004";
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

app.MapControllers();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.Run();
