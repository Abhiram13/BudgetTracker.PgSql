using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category> InsertCategoryAsync(Category payload)
    {
        Category bank = await _categoryRepository.InsertOneCategoryAsync(payload);
        return bank;
    }
}