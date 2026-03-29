using BudgetTracker.Finance;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Finance.Disposals;

public sealed class BankDisposal : IAsyncDisposable
{
    private readonly WriteDbContext _dbContext;

    public BankDisposal(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dbContext.Banks.ExecuteDeleteAsync();
    }
}