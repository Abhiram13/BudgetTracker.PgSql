using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Interfaces;

public interface IBankRepository
{
    Task<Bank> InsertOneBankAsync(Bank payload);
}