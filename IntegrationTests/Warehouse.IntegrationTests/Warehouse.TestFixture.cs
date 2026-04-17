using BudgetTracker.Shared.Constants;
using BudgetTracker.Warehouse.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IntegrationTests.Warehouse.Services;
using IntegrationTests.Warehouse.Factory;

namespace IntegrationTests.Warehouse.Fixtures;

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
            IOptions<WarehouseAppSecrets> wareHouseConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<WarehouseAppSecrets>>();
            SetClientHeaders(wareHouseConfiguration.Value);
        }

        return Task.CompletedTask;
    }
    
    private void SetClientHeaders(WarehouseAppSecrets config)
    {
        string traceId = Guid.NewGuid().ToString();
        string yarpApiKey = "";
        
        Client.DefaultRequestHeaders.Add(HeaderNames.X_TRACE_ID, traceId);
        Client.DefaultRequestHeaders.Add(HeaderNames.YARP_API_KEY, yarpApiKey);
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        return Task.CompletedTask;
    }
}