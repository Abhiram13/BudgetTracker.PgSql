using Microsoft.AspNetCore.Mvc.Testing;
using BudgetTracker.Warehouse;
using Microsoft.AspNetCore.Hosting;
using Abhiram.Extensions.DotEnv;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Warehouse.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.IntegrationTests.Model;
using Warehouse.IntegrationTests.Services;

namespace Warehouse.IntegrationTests.Setup;

public class WarehouseTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        DotEnvironmentVariables.Load();
        
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddSecrets(environment: context.HostingEnvironment, optional: false)
                .AddEnvironmentVariables();
        });

        builder.ConfigureServices((context, services) =>
        {
            services.AddOptions<WarehouseAppSecrets>()
                .Bind(context.Configuration)
                .Bind(context.Configuration.GetSection("BigQuery"))
                .Bind(context.Configuration.GetSection("Secrets"))
                .ValidateOnStart();
            services.AddOptions<GoogleCloudProject>().Bind(context.Configuration).ValidateOnStart();
            services.AddSingleton<WareHouseService>();
        });
    }
}