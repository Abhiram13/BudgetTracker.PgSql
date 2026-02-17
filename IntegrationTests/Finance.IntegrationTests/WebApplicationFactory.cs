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

namespace IntegrationTests;

public class FinanceTestWebApplicationFactory : WebApplicationFactory<Program>
{
    public WriteDbContext GetDbContext()
    {
        return Services.CreateScope().ServiceProvider.GetRequiredService<WriteDbContext>();
    }

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
            config.AddSecrets(environment: context.HostingEnvironment, optional: false);
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            ServiceDescriptor descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<WriteDbContext>));
            services.Remove(descriptor);

            services.AddDbContext<WriteDbContext>(option =>
            {
                option.UseNpgsql(FinanceTestDbContextFactory.GetConnectionString());
            });
        });
    }
}