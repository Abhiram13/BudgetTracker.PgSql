using BudgetTracker.Dues.Entities;
using BudgetTracker.Dues.Interfaces;

namespace BudgetTracker.Dues.Repository;

public class DueRepository : IDueRepository
{
    private readonly WriteDBContext _writeDbContext;

    public DueRepository (WriteDBContext context)
    {
        _writeDbContext = context;
    }

    public async Task InsertDueAsync(Due payload)
    {
        await _writeDbContext.Dues.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
    }
}