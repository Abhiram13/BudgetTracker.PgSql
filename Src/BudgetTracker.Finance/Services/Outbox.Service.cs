using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Services;

public class OutboxService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<OutboxService> _logger; // TODO: Remove, no need for logger in repository

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
}