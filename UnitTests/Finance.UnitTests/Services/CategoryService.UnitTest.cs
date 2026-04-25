using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Repository;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.Finance.Services;

public class CategoryServiceUnitTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new Mock<ICategoryRepository>();
    private readonly CategoryService _categoryService;

    public CategoryServiceUnitTests()
    {
        _categoryService = new CategoryService(_categoryRepository.Object);
    }
    
    [Fact]
    public async Task GetCategoryById_Success_Async()
    {
        // Arrange
        Category category = Category.Create("Food");
        _categoryRepository.Setup(r => r.GetCategoryAsync(1)).ReturnsAsync(category);

        // Act
        Category result = await _categoryService.GetCategoryByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Food", result.Name);
    }
    
    [Fact]
    public async Task GetCategoryById_Throws_Error_Async()
    {
        _categoryRepository
            .Setup(r => r.GetCategoryAsync(0))
            .ThrowsAsync(new BadHttpRequestException("Category not found"));

        // Act
        Func<Task<Category>> action = () => _categoryService.GetCategoryByIdAsync(0);

        // Assert
        Exception error = await Assert.ThrowsAsync<InvalidPayloadException>(action);
        Assert.Equal("Category not found", error.Message);
    }

    [Fact]
    public void Invalid_Category_Name()
    {
        Category category = Category.Create(null);
        
        Assert.Throws<BadHttpRequestException>(() => category);
    }
}