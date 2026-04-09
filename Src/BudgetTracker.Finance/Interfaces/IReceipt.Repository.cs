using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Interfaces;

public interface IReceiptRepository
{
    Task InsertOneAsync(Receipt receipt);
}