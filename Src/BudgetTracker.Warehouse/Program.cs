using System.Net;
using Abhiram.Extensions.DotEnv;
using BudgetTracker.Shared.Middlwares;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Warehouse.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHostedService<SubscriberBackgroundService>();
builder.Services.AddSingleton<BigQueryService>();
builder.Services.AddScoped<TraceIdProvider>();
builder.Services.AddScoped<SubscriberService>();
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
