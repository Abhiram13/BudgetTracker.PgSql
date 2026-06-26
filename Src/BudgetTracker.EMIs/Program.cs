using PostgresWebApiTemplate;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Microsoft.EntityFrameworkCore;
using System.Net;
using BudgetTracker.EMIs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog();
builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<WriteDbContext>(options =>
{
    // TODO: UPDATE EITHER WITH .ENV OR app.settings.json
    options.UseNpgsql(builder.Configuration.GetConnectionString("")!);
});
builder.WebHost.ConfigureKestrel((_, server) =>
{
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3003";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

using (IServiceScope? scope = app.Services.CreateScope())
{
    try
    {
        WriteDbContext context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        context.Database.Migrate();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();
