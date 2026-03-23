using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Shared.Constants;
using IntegrationTests.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IntegrationTests.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Setup;

public class IntegrationTestFixture : IAsyncLifetime
{
    public FinanceTestWebApplicationFactory Factory { get; set; } = default!;
    public HttpClient Client { get; private set; } = default!;
    public Category TestCategory { get; private set; } = default!;
    public Bank TestBank { get; private set; } = default!;
    private CategoryBuilder _categoryBuilder = default!;
    private BankBuilder _bankBuilder = default!;
    
    public async Task InitializeAsync()
    {
        Factory = new FinanceTestWebApplicationFactory();
        Client = Factory.CreateClient();
        
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            await dbContext.Database.EnsureCreatedAsync();
            _categoryBuilder = scope.ServiceProvider.GetRequiredService<CategoryBuilder>();
            _bankBuilder = scope.ServiceProvider.GetRequiredService<BankBuilder>();
            TestCategory = await _categoryBuilder.CreateCategoryAsync();
            TestBank = await _bankBuilder.CreateBankAsync();
            SetClientHeaders(config);
        }
    }

    private void SetClientHeaders(IConfiguration configuration)
    {
        string traceId = Guid.NewGuid().ToString();
        string yarpApiKey = SetYarpConfig(configuration);
        
        Client!.DefaultRequestHeaders.Add("X-Trace-Id", traceId);
        Client.DefaultRequestHeaders.Add(HeaderNames.YARP_API_KEY, yarpApiKey);
    }

    private string SetYarpConfig(IConfiguration configuration)
    {
        string? yarpApiKey = configuration["SECRETS:YARP_API_KEY"];

        if (string.IsNullOrEmpty(yarpApiKey)) throw new NotFoundException($"YARP_API_KEY ({yarpApiKey}) is missing");

        return yarpApiKey;
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