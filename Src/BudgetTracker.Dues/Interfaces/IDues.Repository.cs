using BudgetTracker.Dues.Entities;

namespace BudgetTracker.Dues.Interfaces;

public interface IDueRepository
{
    Task InsertDueAsync(Due payload);
}