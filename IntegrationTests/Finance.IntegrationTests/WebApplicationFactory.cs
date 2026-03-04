using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BudgetTracker.Finance;
using BudgetTracker.Shared.Models;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using IntegrationTests.Builders;
using IntegrationTests.Exceptions;

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

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddSecrets(environment: context.HostingEnvironment, optional: false)
                .AddEnvironmentVariables();
        });

        builder.ConfigureServices((context, services) =>
        {
            ServiceDescriptor descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<WriteDbContext>));
            services.Remove(descriptor);
            services.AddDbContext<WriteDbContext>(option =>
            {
                string connectionString = context.Configuration.GetSection("DbConnectionStrings")["FinanceDb"] 
                                          ?? throw new NotFoundException("FinanceDb connection string not found");
                
                option.UseNpgsql(connectionString);
            });
            services.AddScoped<CategoryBuilder>();
            services.AddScoped<BankBuilder>();
        });
    }
}