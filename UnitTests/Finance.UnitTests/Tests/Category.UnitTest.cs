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
    
    #region Get Category Tests
    
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
    
    #endregion

    #region Insert Category Tests

    [Theory]
    [ClassData(typeof(InsertCategoryData))]
    public async Task Insert_Category_Valid_Data_Async(string categoryName)
    {
        // Arrange
        InsertCategoryDto insertDto = new InsertCategoryDto { Name = categoryName };
    
        // When inserting category in Category repository, it first checks for category by given name and return it or null.
        // So by defaulting setting up null to pass to Insert category.
        _categoryRepository.Setup(c => c.GetCategoryAsync(categoryName)).ReturnsAsync((Category)null);

        // Passing IsAny<Category> here instead of Category.Create() because a 'Category' instance will be created within _categoryService.InsertCategoryAsync()
        // When these two instances of Category is created, 'result' is getting null.
        // So with IsAny<>, we are returning any Category object that was created within _categoryService.InsertCategoryAsync().
        // And here at .ReturnAsync() we are returning that whatever 'Category' instance that was created within.
        _categoryRepository.Setup(c => c.InsertOneCategoryAsync(It.IsAny<Category>())).ReturnsAsync((Category c) => c);
    
        // Act
        Category result = await _categoryService.InsertCategoryAsync(insertDto);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryName, result.Name);
    
        // Verify that the repository was actually called
        // This test feels too much checking???
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