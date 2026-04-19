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
using BudgetTracker.Warehouse.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Transforms;

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
builder.Services.AddSwaggerGen();
builder.Services.LoadJwtConfiguration(builder.Configuration);
builder.Services.AddOptions<GatewayAppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSingleton<GatewayAppSecrets>(option => option.GetRequiredService<IOptions<GatewayAppSecrets>>().Value);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transform =>
    {
        transform.AddRequestTransform(context =>
        {
            RouteModel cluster = context.HttpContext.GetRouteModel();
            string? clusterId = cluster.Config.ClusterId;

            if (string.IsNullOrEmpty(clusterId))
            {
                return ValueTask.CompletedTask; // TODO: Check how to verify cluster id is valid 
            }
            
            // FIX: Since this service is not getting registerd without IOptions<T>, silent gateway error was thrown.
            JwtConfiguration secrets = context.HttpContext.RequestServices.GetRequiredService<IOptions<JwtConfiguration>>().Value;
            secrets.Audience = clusterId;
            string token = JwtFactory.CreateToken(secrets, scope: SharedConstants.Jwt.Scopes.DOWNSTREAM);
            context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            return ValueTask.CompletedTask;
        });
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TraceProviderMiddleware>();
app.UseMiddleware<BadGatewayMiddleware>();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy().RequireAuthorization();
app.UseHttpsRedirection();
app.Run();
