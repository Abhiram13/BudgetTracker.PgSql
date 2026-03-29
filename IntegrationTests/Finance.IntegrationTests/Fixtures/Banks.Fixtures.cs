using BudgetTracker.Finance;
using IntegrationTests.Finance.Factory;
using IntegrationTests.Finance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Finance.Fixtures;

public class BanksTestsFixture : FinanceTestFixture, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            FinanceConfig financeConfig = scope.ServiceProvider.GetRequiredService<IOptions<FinanceConfig>>().Value;
            await dbContext.Database.MigrateAsync();
            SetClientHeaders(financeConfig);
        }
    }

    public async Task DisposeAsync()
    {
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await dbContext.Banks.ExecuteDeleteAsync();
        }
        
        DisposeFactoryAndClient();
    }
}