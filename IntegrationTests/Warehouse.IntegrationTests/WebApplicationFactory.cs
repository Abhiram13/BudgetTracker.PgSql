using Microsoft.AspNetCore.Mvc.Testing;
using BudgetTracker.Warehouse;
using Microsoft.AspNetCore.Hosting;
using Abhiram.Extensions.DotEnv;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.IntegrationTests.Services;

namespace Warehouse.IntegrationTests.Setup;

public class WarehouseTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        DotEnvironmentVariables.Load();

        builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<WareHouseService>();
        });
    }
}