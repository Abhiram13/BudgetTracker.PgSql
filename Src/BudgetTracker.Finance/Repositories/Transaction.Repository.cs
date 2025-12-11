using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly WriteDbContext _writeDbContext;

    public TransactionRepository(WriteDbContext write)
    {
        _writeDbContext = write;
    }

    public async Task<Transaction> InsertOneTransactionAsync(Transaction payload)
    {
        await _writeDbContext.Transactions.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }
}