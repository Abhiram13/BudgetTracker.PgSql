using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public CategoryRepository(WriteDbContext write, ReadDbContext read)
    {
        _writeDbContext = write;
        _readDbContext = read;
    }

    public async Task<Category> InsertOneCategoryAsync(Category payload)
    {
        await _writeDbContext.Categories.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }

    public async Task<List<CategoryDto>> ListOfCategoryAsync()
    {
        List<CategoryDto> list = await _readDbContext.Categories
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

        return list;
    }

    public async Task UpdateOneCategoryAsync(Category payload)
    {
        Category? category = await GetCategoryAsync(payload.Id);

        if (category == null)
        {
            throw new InvalidPayloadException($"Category with ({payload.Id}) not found");
        }
        
        category.Name = payload.Name;
        category.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task<Category?> GetCategoryAsync(int id)
    {
        Category? category = await _readDbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        return category;
    }

    public async Task<Category?> GetCategoryAsync(string categoryName)
    {
        Category? category = await _readDbContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
        return category;
    }
}