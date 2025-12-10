using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Repository;

public class BankRepository : IBankRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public BankRepository(WriteDbContext write, ReadDbContext read)
    {
        _writeDbContext = write;
        _readDbContext = read;
    }

    public async Task<Bank> InsertOneBankAsync(Bank payload)
    {
        await _writeDbContext.Banks.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }
}