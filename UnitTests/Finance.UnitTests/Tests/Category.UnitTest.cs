using System.Data;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Repository;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using UnitTests.Finance.Data.Categories;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace UnitTests.Finance.Tests.Categories;

public class CategoryUnitTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new Mock<ICategoryRepository>();
    private readonly CategoryService _categoryService;

    public CategoryUnitTests()
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
        Assert.Equal("Food", result.Name);
    }
    
    [Fact]
    public async Task GetCategoryById_Throws_Error_Async()
    {
        _categoryRepository.Setup(r => r.GetCategoryAsync(0)).ReturnsAsync((Category) null);

        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _categoryService.GetCategoryByIdAsync(0);
        });
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidPayloadException>(exception);
        Assert.Equal("Category with (0) not found", exception.Message);
    }

    #region Insert Category Tests

    [Theory]
    [ClassData(typeof(InsertCategoryData))]
    public async Task Insert_Category_Valid_Data_Async(string categoryName)
    {
        // Arrange
        InsertCategoryDto insertDto = new InsertCategoryDto { Name = categoryName };
    
        // 1. Ensure the duplicate check returns null
        _categoryRepository.Setup(c => c.GetCategoryAsync(categoryName)).ReturnsAsync((Category)null);

        // 2. Setup the insert to accept ANY Category object and return it back
        _categoryRepository.Setup(c => c.InsertOneCategoryAsync(It.IsAny<Category>())).ReturnsAsync((Category c) => c); 
    
        // Act
        Category result = await _categoryService.InsertCategoryAsync(insertDto);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryName, result.Name);
    
        // Verify that the repository was actually called
        _categoryRepository.Verify(c => c.InsertOneCategoryAsync(It.Is<Category>(x => x.Name == categoryName)), Times.Once);
    }

    [Fact]
    public async Task Insert_Category_Throws_Error_Duplicate_Name_Async()
    {
        // Arrange
        const string categoryName = "Food";
        _categoryRepository.Setup(c => c.GetCategoryAsync(categoryName)).ReturnsAsync(Category.Create(categoryName));

        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _categoryService.InsertCategoryAsync(new InsertCategoryDto { Name = categoryName });
        });
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidPayloadException>(exception);
        Assert.Equal($"Category with name ({categoryName}) already exists", exception.Message);
    }

    [Theory]
    [ClassData(typeof(InsertCategoryInvalidTestData))]
    public async Task Insert_Category_Invalid_Data_Throws_Error_Async(string categoryName)
    {
        _categoryRepository.Setup(c => c.GetCategoryAsync(It.IsAny<string>())).ReturnsAsync((Category) null);
        
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _categoryService.InsertCategoryAsync(new InsertCategoryDto { Name = categoryName });
        });
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidPayloadException>(exception);
    }

    #endregion
}