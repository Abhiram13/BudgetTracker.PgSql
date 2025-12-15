using System.Net;
using BudgetTracker.Gateway.Middlewares;
using Abhiram.Extensions.DotEnv;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
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

app.MapReverseProxy();
app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<BadGatewayMiddleware>();
app.UseMiddleware<TraceProviderMiddleware>();
app.Run();
