using BudgetTracker.Finance.Interfaces;

namespace BudgetTracker.Finance.Repository;

public class OutboxRepository : IOutboxRepository
{
    private readonly WriteDbContext _writeDbContext;

    public OutboxRepository(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }
    
    public async Task InsertOneAsync(FinanceOutboxEvents outboxPayload)
    {
        DateTime today = DateTime.UtcNow;
        
        outboxPayload.Id = Guid.NewGuid();
        outboxPayload.CreatedAt = today;
        outboxPayload.UpdatedAt = today;
        
        await _writeDbContext.AddAsync(outboxPayload);
        await _writeDbContext.SaveChangesAsync();
    }
}