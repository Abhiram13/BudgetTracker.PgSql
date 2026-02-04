using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

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

    public async Task<List<CategoryListDto>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.ListOfCategoryAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.SearchByIdAsync(id);
    }
}