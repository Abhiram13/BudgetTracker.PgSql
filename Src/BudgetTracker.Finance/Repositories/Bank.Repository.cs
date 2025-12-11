using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Repository;

public class BankRepository : IBankRepository
{
    private readonly WriteDbContext _writeDbContext;

    public BankRepository(WriteDbContext write)
    {
        _writeDbContext = write;
    }

    public async Task<Bank> InsertOneBankAsync(Bank payload)
    {
        await _writeDbContext.Banks.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }
}