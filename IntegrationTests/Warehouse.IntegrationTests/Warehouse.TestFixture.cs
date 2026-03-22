using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Warehouse.IntegrationTests.Services;
using Warehouse.IntegrationTests.Model;

namespace Warehouse.IntegrationTests.Setup;

public class WarehouseIntegrationTestFixture : IAsyncLifetime
{
    public WarehouseTestWebApplicationFactory Factory { get; private set; } = default!;
    public HttpClient Client { get; private set; } = default!;
    public HttpClient UnAuthorizedClient { get; private set; } = default!;
    public WareHouseService? WarehouseService { get; private set; }
    
    public Task InitializeAsync()
    {
        Factory = new WarehouseTestWebApplicationFactory();
        Client = Factory.CreateClient();
        UnAuthorizedClient = Factory.CreateClient();

        using (IServiceScope scope = Factory.Services.CreateScope())
        {
            WarehouseService = scope.ServiceProvider.GetRequiredService<WareHouseService>();
            IOptions<WareHouseConfiguration> wareHouseConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<WareHouseConfiguration>>();
            SetClientHeaders(wareHouseConfiguration.Value);
        }

        return Task.CompletedTask;
    }
    
    private void SetClientHeaders(WareHouseConfiguration config)
    {
        string traceId = Guid.NewGuid().ToString();
        string yarpApiKey = config.YarpApiKey;
        
        Client.DefaultRequestHeaders.Add("X-Trace-Id", traceId);
        Client.DefaultRequestHeaders.Add("YARP_API_KEY", yarpApiKey);
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        return Task.CompletedTask;
    }
}