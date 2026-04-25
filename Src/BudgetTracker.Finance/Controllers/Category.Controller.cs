using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using BudgetTracker.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BudgetTracker.Finance.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = SharedConstants.Jwt.Policies.DOWNSTREAM_POLICY)]
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

    [HttpPost(Name = "INSERT_CATEGORY")]
    public async Task<ActionResult<ApiResponse<string>>> InsertAsync([FromBody] InsertCategoryDto payload)
    {
        await _categoryService.InsertCategoryAsync(payload);
        return StatusCode(201, new ApiResponse
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId = _traceIdProvider.TraceId,
            Message = "Category created successfully"
        });
    }

    [HttpGet(Name = "GET_ALL_CATEGORIES")]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategoriesAsync()
    {
        List<CategoryDto> list = await _categoryService.GetAllCategoriesAsync();
        return Ok(new ApiResponse<List<CategoryDto>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceIdProvider.TraceId,
            Result = list
        });
    }

    [HttpGet("{id}", Name = "GET_CATEGORY_BY_ID")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        Category category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(new ApiResponse<CategoryDto>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceIdProvider.TraceId,
            Result = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            }
        });
    }

    [HttpPut("{id}", Name = "UPDATE_CATEGORY_BY_ID")]
    public async Task<IActionResult> UpdateOnAsync([FromRoute] int id, [FromBody] InsertCategoryDto payload)
    {
        await _categoryService.UpdateCategoryAsync(id, payload.Name);

        return Ok(new ApiResponse
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceIdProvider.TraceId,
            Message = "Category updated successfully"
        });
    }
}