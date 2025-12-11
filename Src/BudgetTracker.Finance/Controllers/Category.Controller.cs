using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Finance.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly TraceIdProvider _traceProvider;

    public CategoryController(CategoryService categoryService, TraceIdProvider trace)
    {
        _categoryService = categoryService;
        _traceProvider = trace;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> InsertAsync([FromBody] InsertCategoryDto payload)
    {
        DateTime now = DateTime.UtcNow;
        Category category = new Category
        {
            CreatedAt = now,
            UpdatedAt = now,
            Name = payload.Name
        };

        await _categoryService.InsertCategoryAsync(category);
        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId = _traceProvider.TraceId,
            Message = "Category created successfully"
        });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryListDto>>>> GetCategoriesAsync()
    {
        List<CategoryListDto> list = await _categoryService.GetAllCategoriesAsync();
        return Ok(new ApiResponse<List<CategoryListDto>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = list
        });
    }
}