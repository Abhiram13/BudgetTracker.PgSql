using BudgetTracker.Finance;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Finance.Disposals;

public sealed class DueDisposal : IAsyncDisposable
{
    private readonly WriteDbContext _dbContext;

    public DueDisposal(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dbContext.Dues.ExecuteDeleteAsync();
    }
}