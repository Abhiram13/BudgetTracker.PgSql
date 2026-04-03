using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface IOutboxRepository
{
    Task InsertOneAsync(FinanceOutboxEvents outboxPayload);
    Task<List<OutboxUnProcessedDto>> GetUnProcessedMessagesAsync(int maxLimit);
    Task UpdateCountAndErrorAsync(Guid id, string errorMessage);
    Task UpdateSuccessStatusAsync(Guid id);
}