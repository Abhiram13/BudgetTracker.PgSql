namespace BudgetTracker.Finance.Interfaces;

public interface IOutboxRepository
{
    Task InsertOneAsync(FinanceOutboxEvents outboxPayload);
}