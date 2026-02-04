using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface ICategoryRepository
{
    Task<Category> InsertOneCategoryAsync(Category payload);
    Task<List<CategoryListDto>> ListOfCategoryAsync();
    Task UpdateOneCategoryAsync(Category payload);
    Task<Category> SearchByIdAsync(int id);
}