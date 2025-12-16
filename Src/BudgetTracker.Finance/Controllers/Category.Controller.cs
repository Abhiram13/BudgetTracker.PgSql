using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using BudgetTracker.Shared.Security;

namespace BudgetTracker.Finance.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = YarpApiKeySchemaOptions.DefaultSchema)]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly string _traceId = "";

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
        _traceId = Request.Headers["X-Trace-Id"]!;
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
            TraceId = _traceId,
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
            TraceId = _traceId,
            Result = list
        });
    }
}