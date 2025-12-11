using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> InsertOneTransactionAsync(Transaction payload);
}