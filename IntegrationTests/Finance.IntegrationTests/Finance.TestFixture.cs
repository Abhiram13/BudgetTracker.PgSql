using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Shared.Constants;
using Finance.IntegrationTests.Models;
using IntegrationTests.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Setup;

public class IntegrationTestFixture : IAsyncLifetime
{
    public FinanceTestWebApplicationFactory Factory { get; set; } = default!;
    public HttpClient Client { get; private set; } = default!;
    public HttpClient UnauthorizedClient { get; private set; } = default!;
    public Category TestCategory { get; private set; } = default!;
    public Bank TestBank { get; private set; } = default!;
    private CategoryBuilder _categoryBuilder = default!;
    private BankBuilder _bankBuilder = default!;
    
    public async Task InitializeAsync()
    {
        Factory = new FinanceTestWebApplicationFactory();
        Client = Factory.CreateClient();
        UnauthorizedClient = Factory.CreateClient();
        
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            FinanceConfig financeConfig = scope.ServiceProvider.GetRequiredService<IOptions<FinanceConfig>>().Value;
            await dbContext.Database.EnsureCreatedAsync();
            _categoryBuilder = scope.ServiceProvider.GetRequiredService<CategoryBuilder>();
            _bankBuilder = scope.ServiceProvider.GetRequiredService<BankBuilder>();
            TestCategory = await _categoryBuilder.CreateCategoryAsync();
            TestBank = await _bankBuilder.CreateBankAsync();
            SetClientHeaders(financeConfig);
        }
    }

    private void SetClientHeaders(FinanceConfig configuration)
    {
        string traceId = Guid.NewGuid().ToString();
        string yarpApiKey = configuration.Secrets.YarpApiKey;
        
        Client.DefaultRequestHeaders.Add(HeaderNames.X_TRACE_ID, traceId);
        Client.DefaultRequestHeaders.Add(HeaderNames.YARP_API_KEY, yarpApiKey);
    }

    public async Task DisposeAsync()
    {
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await dbContext.Categories.ExecuteDeleteAsync();
            await dbContext.Banks.ExecuteDeleteAsync();
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE categories RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE banks RESTART IDENTITY CASCADE");
        }
        
        Client.Dispose();
        Factory.Dispose();
    }
}