using System.Net;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Abhiram.Extensions.DotEnv;
using Abhiram.Abstractions.Logging;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Extensions;
using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Security;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

string baseDir = AppContext.BaseDirectory;
string environment = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile(Path.Combine(baseDir, "sharedsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(baseDir, $"sharedsettings.{environment}.json"), optional: false, reloadOnChange: true);

builder.AddConsoleGoogleSeriLog();
builder.Configuration.AddSecrets(environment: builder.Environment, optional: false);
builder.Services.AddOptions<AppSecrets>().Bind(builder.Configuration).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddCollections();
builder.Services.AddSwaggerGen(sw =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(baseDir, xmlFile);
    sw.IncludeXmlComments(xmlPath);
    
    string sharedXml = "BudgetTracker.Shared.xml"; 
    string sharedPath = Path.Combine(AppContext.BaseDirectory, sharedXml);
    
    if (File.Exists(sharedPath))
    {
        sw.IncludeXmlComments(sharedPath);
    }
    
    sw.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Budget Tracker Finance API",
        Version = "v1",
        Description = "Comprehensive APIs for managing bank transactions and categories."
    });
    
    const string SWAGGER_API_SCHEMA = "Yarp-Api-Key";
    sw.AddSecurityDefinition(SWAGGER_API_SCHEMA, new OpenApiSecurityScheme
    {
        Description = "Yarp api key that gets passed and authenticated to downstream apis",
        Name = HeaderNames.YARP_API_KEY,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = YarpApiKeySchemaOptions.DefaultSchema,
    });
    
    sw.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { 
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = SWAGGER_API_SCHEMA, Type = ReferenceType.SecurityScheme }},
            Array.Empty<string>()
        }
    });
});

builder.WebHost.ConfigureKestrel((_, server) => {
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3001";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("DB Migration is starting...");
        
        PostgresSecrets secrets = scope.ServiceProvider.GetRequiredService<AppSecrets>().Postgres;
        DbContextOptionsBuilder<WriteDbContext> contextOptionsBuilder = new DbContextOptionsBuilder<WriteDbContext>();
        string connectionString = $"Host={secrets.Host};Port={secrets.MigratePort};Database={secrets.Database};Username={secrets.MigrateUsername};Password={secrets.MigratePassword}";
        contextOptionsBuilder.UseNpgsql(connectionString);

        using (WriteDbContext writeDbContext = new WriteDbContext(contextOptionsBuilder.Options))
        {
            writeDbContext.Database.Migrate();
        }
        
        logger.LogInformation("DB Migration completed");
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "Exception at Finance Server DB Migrate Setup - {ErrorMessage}", e.InnerException?.Message ?? e.Message);
    }
}

app.UseApplicationServices();
app.Run();

namespace BudgetTracker.Finance
{
    public partial class Program { }
}