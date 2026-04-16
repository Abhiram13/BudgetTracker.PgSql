using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Exceptions;

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
        Category? category = await _categoryRepository.GetCategoryAsync(payload.Name);
        
        if (category != null)
        {
            throw new InvalidPayloadException($"Category with name ({payload.Name}) already exists");
        }
        
        return await _categoryRepository.InsertOneCategoryAsync(payload);
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.ListOfCategoryAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        Category? category = await _categoryRepository.GetCategoryAsync(id);

        if (category == null)
        {
            throw new InvalidPayloadException($"Category with ({id}) not found");
        }
        
        return category;
    }

    public async Task UpdateCategoryAsync(Category payload)
    {
        await _categoryRepository.UpdateOneCategoryAsync(payload);
    }
}