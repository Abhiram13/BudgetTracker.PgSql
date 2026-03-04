using BudgetTracker.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Setup;

public sealed class FinanceDbDisposal : IAsyncDisposable
{
    private readonly WriteDbContext _dbContext;

    public FinanceDbDisposal(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dbContext.Transactions.ExecuteDeleteAsync();
    }
}