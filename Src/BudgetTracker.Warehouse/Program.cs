using System.Net;
using Microsoft.Extensions.Options;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Middlwares;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Warehouse.Models;
using BudgetTracker.Warehouse.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog(template: "[{Level:u3}] [Source: {SourceContext}] {Message:lj}{NewLine}{Exception}");
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOptions<WarehouseAppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSingleton<WarehouseAppSecrets>(sp => sp.GetRequiredService<IOptions<WarehouseAppSecrets>>().Value);
builder.Services.AddSingleton<YarpApiKeySecret>(sp => sp.GetRequiredService<IOptions<WarehouseAppSecrets>>().Value.Secrets);
builder.Services.AddSingleton<BigQueryService>();
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddScoped<SubscriberService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = YarpApiKeySchemaOptions.DefaultSchema;
    options.DefaultChallengeScheme = YarpApiKeySchemaOptions.DefaultSchema;
}).AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});
builder.Services.AddHostedService<SubscriberBackgroundService>(); // TODO: This step blocking application shutdown

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
