using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;

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
}