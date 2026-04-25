using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;

namespace IntegrationTests.Finance.Builders;

public sealed class CategoryBuilder
{
    private readonly WriteDbContext _dbContext;

    public CategoryBuilder(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category> CreateCategoryAsync()
    {
        Category category = Category.Create("Test category");
        
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
        
        return category;
    }
}