using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly WriteDbContext _writeDbContext;

    public CategoryRepository(WriteDbContext write)
    {
        _writeDbContext = write;
    }

    public async Task<Category> InsertOneCategoryAsync(Category payload)
    {
        await _writeDbContext.Categories.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }

    public async Task<List<CategoryListDto>> ListOfCategoryAsync()
    {
        List<CategoryListDto> list = await _writeDbContext.Categories
            .Select(c => new CategoryListDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

        return list;
    }

    public async Task UpdateOneCategoryAsync(Category payload)
    {
        Category category = await SearchByIdAsync(payload.Id);
        
        category.Name = payload.Name;
        category.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task<Category> SearchByIdAsync(int id)
    {
        Category? category = await _writeDbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        
        if (category is null) throw new BadHttpRequestException("Category not found");
        
        return category;
    }
}