using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BudgetTracker.Finance;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Finance.Configurations;
using BudgetTracker.Finance.Extensions;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Extensions;
using Google.Api;
using Google.Cloud.PubSub.V1;
using IntegrationTests.Finance.Models;
using IntegrationTests.Finance.Builders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Npgsql;
using Encoding = System.Text.Encoding;

namespace IntegrationTests.Finance.Factory;

public class FinanceTestWebApplicationFactory : WebApplicationFactory<Program>
{
    public IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        DotEnvironmentVariables.Load();
        
        builder.UseEnvironment("Test");

        // loading secrets from .env and appsettings.<env>.json into builder.configuration
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddSecrets(environment: context.HostingEnvironment, optional: false)
                .AddEnvironmentVariables();
        });

        // Removing default WriteDbContext from main application and replacing it with test DbContext.
        // Also registering services into Scoped lifetime
        builder.ConfigureServices((context, services) =>
        {
            // loading all .env, appsettings.json and appsettings.<env>.json configs in FinanceConfig
            services.AddOptions<FinanceConfig>().Bind(context.Configuration).ValidateOnStart();
            ServiceDescriptor descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<WriteDbContext>));
            ServiceDescriptor readContextDescriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<ReadDbContext>));
            ServiceDescriptor migrateContextDescriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<MigrateDbContext>));
            ServiceDescriptor pubSubSubscriberClientDescriptor = services.Single(s => s.ServiceType == typeof(SubscriberClient));
            
            services.Remove(descriptor);
            services.Remove(readContextDescriptor);
            services.Remove(migrateContextDescriptor);
            services.Remove(pubSubSubscriberClientDescriptor);

            services
                .AddDatabaseConfiguration()
                .LoadJwtConfiguration(context.Configuration)
                .AddScoped<NpgsqlConnection>(provider =>
                {
                    DatabaseConfiguration dbConfig = provider.GetRequiredService<IOptionsMonitor<DatabaseConfiguration>>().Get(DatabaseType.WRITE);
                    string? postgresHost = dbConfig.Host;
                    string? postgresPort = dbConfig.Port;
                    string? postgresDatabase = dbConfig.Database;
                    string? postgresUsername = dbConfig.Username;
                    string? postgresPassword = dbConfig.Password;
                    string connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";

                    NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                    connection.Open();
                    return connection;
                })
                .AddPostgresDbContext<WriteDbContext>()
                .AddPostgresDbContext<ReadDbContext>()
                .AddPostgresDbContext<MigrateDbContext>()
                .AddScoped<CategoryBuilder>()
                .AddScoped<BankBuilder>()
                .AddScoped<DueBuilder>()
                .AddSingleton<SubscriberClient>(_ => new Mock<SubscriberClient>().Object)
                .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme) // overriding server jwt config
                .PostConfigure<IOptions<JwtConfiguration>>((options, config) =>
                {
                    JwtConfiguration secrets = config.Value;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = secrets.Issuer,
                        ValidateAudience = true,
                        ValidAudience = secrets.Audience,
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrets.SigningKey)),
                    };
                });
        });
    }
}