using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BudgetTracker.Finance;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using Finance.IntegrationTests.Models;
using IntegrationTests.Builders;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Setup;

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
            services.AddOptions<FinanceConfig>().Bind(context.Configuration).ValidateOnStart();
            ServiceDescriptor descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<WriteDbContext>));
            services.Remove(descriptor);
            services.AddDbContext<WriteDbContext>((provider, option) =>
            {
                FinanceConfig config = provider.GetRequiredService<IOptions<FinanceConfig>>().Value;
                option.UseNpgsql(config.DatabaseConnection.FinanceDb);
            });
            services.AddScoped<CategoryBuilder>();
            services.AddScoped<BankBuilder>();
        });
    }
}