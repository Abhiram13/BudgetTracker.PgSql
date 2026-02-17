using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;

namespace IntegrationTests.Builders;

public sealed class CategoryBuilder
{
    private readonly WriteDbContext _dbContext;

    public CategoryBuilder(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category> CreateCategoryAsync()
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        Category category = new Category
        {
            Name = "Test category",
            CreatedAt = date,
            UpdatedAt = date,
        };
        
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
        
        return category;
    }
}