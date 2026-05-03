using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Constants;
using IntegrationTests.Finance.Models;
using IntegrationTests.Finance.Factory;
using IntegrationTests.Finance.Builders;

namespace IntegrationTests.Finance.Fixtures;

/// <summary>
/// Fixture used in Transactions tests
/// <list type="bullet">
///     <item><description>Sets up Auth, UnAuth Http clients</description></item>
///     <item><description>Sets up category, bank builders to manually insert in DB</description></item>
///     <item><description>Creates <see cref="FinanceTestWebApplicationFactory"/>, Http clients and sets up DB and API headers</description></item>
///     <item><description>Disposes category and bank data and disposes DB</description></item>
/// </list>
/// </summary>
/// <remarks><see cref="IDisposable"/></remarks>
public class TransactionsIntegrationTestFixture : FinanceTestFixture, IAsyncLifetime
{
    public Category TestCategory { get; private set; } = default!;
    public Bank TestBank { get; private set; } = default!;
    public Due TestDue { get; private set; } = default!;
    private CategoryBuilder _categoryBuilder = default!;
    private BankBuilder _bankBuilder = default!;
    private DueBuilder _dueBuilder = default!;
    
    public TransactionsIntegrationTestFixture(FinanceTestWebApplicationFactory factory) : base(factory) { }
    
    public async Task InitializeAsync()
    {
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            JwtConfiguration jwtConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<JwtConfiguration>>().Value;
            await dbContext.Database.MigrateAsync();
            _categoryBuilder = scope.ServiceProvider.GetRequiredService<CategoryBuilder>();
            _bankBuilder = scope.ServiceProvider.GetRequiredService<BankBuilder>();
            _dueBuilder = scope.ServiceProvider.GetRequiredService<DueBuilder>();
            TestCategory = await _categoryBuilder.CreateCategoryAsync();
            TestBank = await _bankBuilder.CreateBankAsync();
            TestDue = await _dueBuilder.CreateDueAsync();
            SetClientHeaders(jwtConfiguration);
        }
    }

    public async Task DisposeAsync()
    {
        using (IServiceScope scope = Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await dbContext.Categories.ExecuteDeleteAsync();
            await dbContext.Banks.ExecuteDeleteAsync();
            await dbContext.Dues.ExecuteDeleteAsync();
            
            // since hard-coded banks & category ids "1" and "2" are used in transaction tests, resetting the banks & category table identity.
            // If not every transactions tests access new dynamic bank or category id
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE categories RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE banks RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE dues RESTART IDENTITY CASCADE");
        }
        
        DisposeClients();
    }
}