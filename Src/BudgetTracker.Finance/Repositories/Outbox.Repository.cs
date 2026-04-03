using System.Linq.Expressions;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Repository;

public class OutboxRepository : IOutboxRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public OutboxRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
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

    public async Task<List<OutboxUnProcessedDto>> GetUnProcessedMessagesAsync(int maxLimit)
    {
        DateTime now = DateTime.UtcNow;
        
        // TODO: Pull this counts and minutes from AppSettings
        List<OutboxUnProcessedDto> unProcessedData = await _readDbContext.FinanceOutboxEvents
            .Where(o => o.Status == OutboxStatus.PENDING || (o.Status == OutboxStatus.FAILED && (
                (o.RetryCount == 1 && o.UpdatedAt <= now.AddSeconds(-30)) ||
                (o.RetryCount == 2 && o.UpdatedAt <= now.AddMinutes(-5)) ||
                (o.RetryCount == 3 && o.UpdatedAt <= now.AddMinutes(-30))
            )))
            .OrderBy(o => o.CreatedAt)
            .Take(maxLimit)
            .Select(o => new OutboxUnProcessedDto { Id = o.Id, Payload = o.Payload})
            .ToListAsync();
        
        return unProcessedData;
    }

    public async Task UpdateCountAndErrorAsync(Guid id, string errorMessage)
    {
        DateTime today = DateTime.UtcNow;
        FinanceOutboxEvents? outboxEvent = await _writeDbContext.FinanceOutboxEvents.Where(o =>  o.Id == id).FirstOrDefaultAsync();

        if (outboxEvent is not null)
        {
            outboxEvent.UpdatedAt = today;
            outboxEvent.RetryCount += 1;
            outboxEvent.Error = errorMessage;
            outboxEvent.Status = OutboxStatus.FAILED;
            
            await _writeDbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateSuccessStatusAsync(Guid id)
    {
        FinanceOutboxEvents outboxEvent = await _writeDbContext.FinanceOutboxEvents.FirstAsync(o => o.Id == id);

        outboxEvent.UpdatedAt = DateTime.UtcNow;
        outboxEvent.Status = OutboxStatus.SUCCESS;
        outboxEvent.RetryCount = 0;
        outboxEvent.Error = null;
        outboxEvent.ProcessedAt = DateTime.UtcNow;
            
        await _writeDbContext.SaveChangesAsync();
    }
}