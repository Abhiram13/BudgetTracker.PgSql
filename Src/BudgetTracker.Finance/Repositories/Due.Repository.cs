using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<DueListDetailsDto>> GetListOfDuesAsync()
    {
        List<DueListDetailsDto> result = await _readDbContext.Dues
            .Select(d => new DueListDetailsDto
            {
                Creditor = d.Creditor,
                Debtor = d.Debtor,
                Id = d.Id,
            }).ToListAsync();
        
        return result;
    }

    public async Task<bool> IsDueExistAsync(int dueId)
    {
        int count = await _readDbContext.Dues.CountAsync(d => d.Id == dueId);
        return count > 0;
    }
}