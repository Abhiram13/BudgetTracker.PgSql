using BudgetTracker.Finance;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Finance.Disposals;

public sealed class CategorysDisposal : IAsyncDisposable
{
    private readonly WriteDbContext _dbContext;

    public CategorysDisposal(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dbContext.Categories.ExecuteDeleteAsync();
    }
}