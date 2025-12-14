using System.Net;
using Abhiram.Extensions.DotEnv;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Warehouse.Services;
using Google.Cloud.BigQuery.V2;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<BigQueryService>();
builder.Services.AddScoped<TraceIdProvider>();
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
app.UseHttpsRedirection();
app.Run();
