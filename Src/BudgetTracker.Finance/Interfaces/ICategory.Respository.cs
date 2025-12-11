using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Interfaces;

public interface ICategoryRepository
{
    Task<Category> InsertOneCategoryAsync(Category payload);
}