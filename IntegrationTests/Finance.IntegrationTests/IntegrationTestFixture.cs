using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using IntegrationTests.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IntegrationTests.Exceptions;

namespace IntegrationTests.Setup;

public class IntegrationTestFixture : IAsyncLifetime
{
    private FinanceTestWebApplicationFactory _factory { get; set; } = default!;
    public HttpClient Client { get; private set; }
    public Category TestCategory { get; private set; } = default!;
    public Bank TestBank { get; private set; } = default!;
    
    public async Task InitializeAsync()
    {
        _factory = new FinanceTestWebApplicationFactory();
        Client = _factory.CreateClient();
        
        using (IServiceScope scope = _factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            await dbContext.Database.EnsureCreatedAsync();
            TestCategory = await new CategoryBuilder(dbContext).CreateCategoryAsync();
            TestBank = await new BankBuilder(dbContext).CreateBankAsync();
            SetClientHeaders(config);
        }
    }

    private void SetClientHeaders(IConfiguration configuration)
    {
        string traceId = Guid.NewGuid().ToString();
        string yarpApiKey = SetYarpConfig(configuration);
        
        Client!.DefaultRequestHeaders.Add("X-Trace-Id", traceId);
        Client.DefaultRequestHeaders.Add("YARP_API_KEY", yarpApiKey);
    }

    private string SetYarpConfig(IConfiguration configuration)
    {
        string? yarpApiKey = configuration["SECRETS:YARP_API_KEY"];

        if (string.IsNullOrEmpty(yarpApiKey)) throw new NotFoundException($"YARP_API_KEY ({yarpApiKey}) is missing");

        return yarpApiKey;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        _factory.Dispose();
        
        return Task.CompletedTask;
    }
}