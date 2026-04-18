using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BudgetTracker.Finance;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Shared.Models;
using Google.Api;
using Google.Cloud.PubSub.V1;
using IntegrationTests.Finance.Models;
using IntegrationTests.Finance.Builders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Moq;
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
        
        builder.UseEnvironment("Development");

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
            ServiceDescriptor pubSubSubscriberClientDescriptor = services.Single(s => s.ServiceType == typeof(SubscriberClient));
            
            services.Remove(descriptor);
            services.Remove(readContextDescriptor);
            services.Remove(pubSubSubscriberClientDescriptor);
            
            // Using Same one test DB credentials for Write and Read DBs
            services.AddDbContext<WriteDbContext>((provider, option) =>
            {
                FinanceConfig config = provider.GetRequiredService<IOptions<FinanceConfig>>().Value;
                option.UseNpgsql(config.DatabaseConnection.FinanceDb);
            });
            
            // Using Same one test DB credentials for Write and Read DBs
            services.AddDbContext<ReadDbContext>((provider, option) =>
            {
                FinanceConfig config = provider.GetRequiredService<IOptions<FinanceConfig>>().Value;
                option.UseNpgsql(config.DatabaseConnection.FinanceDb);
            });
            
            // Using Same one test DB credentials for Migrate DBs
            services.AddDbContext<MigrateDbContext>((provider, option) =>
            {
                FinanceConfig config = provider.GetRequiredService<IOptions<FinanceConfig>>().Value;
                option.UseNpgsql(config.DatabaseConnection.FinanceDb);
            });
            services.AddScoped<CategoryBuilder>();
            services.AddScoped<BankBuilder>();
            
            services.AddSingleton<SubscriberClient>(_ => new Mock<SubscriberClient>().Object);

            // overriding server jwt config
            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .PostConfigure<IOptions<FinanceConfig>>((options, config) =>
                {
                    JwtSecret jwtSecrets = config.Value.JwtSecret;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "test-issuer",
                        ValidateAudience = true,
                        ValidAudience = jwtSecrets.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecrets.Key)),
                    };
                });
        });
    }
}