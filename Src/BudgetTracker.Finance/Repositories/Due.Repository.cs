using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;

namespace BudgetTracker.Finance.Repository;

public sealed class DueRepository : IDueRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public DueRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }
    
    public async Task InsertOneAsync(Due payload)
    {
        await _writeDbContext.Dues.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
    }
}