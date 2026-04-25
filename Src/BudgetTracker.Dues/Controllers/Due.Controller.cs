using BudgetTracker.Dues.Entities;
using BudgetTracker.Dues.Models;
using BudgetTracker.Dues.Services;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Dues.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = YarpApiKeySchemaOptions.DefaultSchema)]
[Route("api/dues")]
public class DueController : ControllerBase
{
    private readonly ILogger<DueController> _logger;
    private readonly DueService _dueService;
    private readonly TraceIdProvider _traceProvider;

    public DueController(ILogger<DueController> logger, DueService dueService, TraceIdProvider traceIdProvider)
    {
        _logger = logger;
        _dueService = dueService;
        _traceProvider = traceIdProvider;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> InsertDueAsync([FromBody] InsertDueDto payload)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        Due due = new Due
        {
            Comments = payload.Comments ?? "",
            // CreatedAt = today,
            Creditor = payload.Creditor,
            Debtor = payload.Debtor,
            Description = payload.Description,
            DueAmount = payload.TotalAmount,
            StartDate = DateOnly.Parse(payload.StartDate),
            Status = Enums.DueType.Active,
            Title = payload.Title,
            TotalAmount = payload.TotalAmount,
            // UpdatedAt = today,            
        };

        await _dueService.InsertOneAsync(due);
        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId =_traceProvider.TraceId,
            Message = "Due created successfully"
        });
    }
}