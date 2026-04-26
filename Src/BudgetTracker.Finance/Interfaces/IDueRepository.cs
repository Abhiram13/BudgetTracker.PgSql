using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Interfaces;

public interface IDueRepository
{
    Task InsertOneAsync(Due payload);
}