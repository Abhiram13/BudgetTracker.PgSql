using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface ICategoryRepository
{
    Task<Category> InsertOneCategoryAsync(Category payload);
    Task<List<CategoryDto>> ListOfCategoryAsync();
    Task UpdateOneCategoryAsync(int id, string categoryName);
    Task<Category?> GetCategoryAsync(int id);
    Task<Category?> GetCategoryAsync(string categoryName);
}