using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Services;

public class OutboxService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(IOutboxRepository outboxRepository, ILogger<OutboxService> logger)
    {
        _outboxRepository = outboxRepository;
        _logger = logger;
    }

    public async Task InsertOneAsync(OutboxInsertDto outboxPayload)
    {
        FinanceOutboxEvents financeOutboxEvents = new FinanceOutboxEvents
        {
            EntityId = outboxPayload.EntityId,
            EntityType = outboxPayload.EntityType,
            EventType = outboxPayload.EventType,
            Payload = outboxPayload.Payload,
            Status = outboxPayload.Status,
        };
        
        await _outboxRepository.InsertOneAsync(financeOutboxEvents);
    }

    public async Task<List<OutboxUnProcessedDto>> GetProcessingMessagesAsync(int maxLimit)
    {
        return await _outboxRepository.GetUnProcessedMessagesAsync(maxLimit);
    }

    public async Task UpdateCountAndErrorAsync(Guid id, string errorMessage)
    {
        await _outboxRepository.UpdateCountAndErrorAsync(id, errorMessage);
    }

    public async Task UpdateSuccessAsync(Guid id)
    {
        await _outboxRepository.UpdateSuccessStatusAsync(id);
    }
}