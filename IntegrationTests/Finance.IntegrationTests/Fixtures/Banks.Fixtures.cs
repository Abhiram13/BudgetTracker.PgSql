using BudgetTracker.Finance;
using BudgetTracker.Shared.Configurations;
using IntegrationTests.Finance.Factory;
using IntegrationTests.Finance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Finance.Fixtures;

public class BanksTestsFixture : FinanceTestFixture, IAsyncLifetime
{
    public BanksTestsFixture(FinanceTestWebApplicationFactory factory) : base(factory) { }
    
    public async Task InitializeAsync()
    {
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            JwtConfiguration jwtConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<JwtConfiguration>>().Value;
            await dbContext.Database.MigrateAsync();
            await TruncateTables(dbContext);
            SetClientHeaders(jwtConfiguration);
        }
    }

    public async Task DisposeAsync()
    {
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await dbContext.Banks.ExecuteDeleteAsync();
        }
        
        DisposeClients();
    }
}