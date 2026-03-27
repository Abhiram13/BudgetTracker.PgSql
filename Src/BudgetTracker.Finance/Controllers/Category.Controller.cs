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
    private readonly TraceIdProvider _traceIdProvider;

    public CategoryController(CategoryService categoryService, TraceIdProvider traceIdProvider)
    {
        _categoryService = categoryService;
        _traceIdProvider = traceIdProvider;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> InsertAsync([FromBody] InsertCategoryDto payload)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        Category category = new Category
        {
            CreatedAt = today,
            UpdatedAt = today,
            Name = payload.Name
        };

        await _categoryService.InsertCategoryAsync(category);
        return StatusCode(201, new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId = _traceIdProvider.TraceId,
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
            TraceId = _traceIdProvider.TraceId,
            Result = list
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        Category category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(new ApiResponse<CategoryByIdResponseDto>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceIdProvider.TraceId,
            Result = new CategoryByIdResponseDto
            {
                Id = category.Id,
                Name = category.Name
            }
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOnAsync([FromRoute] int id, [FromBody] InsertCategoryDto payload)
    {
        await _categoryService.UpdateCategoryAsync(new Category { Id = id, Name = payload.Name });

        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceIdProvider.TraceId,
            Message = "Category updated successfully"
        });
    }
}