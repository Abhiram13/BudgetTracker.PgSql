using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;

namespace BudgetTracker.Finance.Repository;

public class ReceiptRepository : IReceiptRepository
{
    private readonly WriteDbContext _writeDbContext;

    public ReceiptRepository(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }
        
    public async Task InsertOneAsync(Receipt receipt)
    {
        await _writeDbContext.Receipts.AddAsync(receipt);
        await _writeDbContext.SaveChangesAsync();
    }
}