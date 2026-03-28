using BudgetTracker.Finance;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Finance.Disposals;

public sealed class CategoryDisposal : IAsyncDisposable
{
    private readonly WriteDbContext _dbContext;

    public CategoryDisposal(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dbContext.Categories.ExecuteDeleteAsync();
    }
}