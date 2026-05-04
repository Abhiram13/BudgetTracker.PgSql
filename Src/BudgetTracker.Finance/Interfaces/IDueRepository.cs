using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface IDueRepository
{
    Task InsertOneAsync(Due payload);
    Task<List<DueListDetailsDto>> GetListOfDuesAsync();
    Task<bool> IsDueExistAsync(int dueId);
}