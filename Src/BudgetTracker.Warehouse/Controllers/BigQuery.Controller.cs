using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Warehouse.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Warehouse.Controllers;

[ApiController]
[Authorize]
[Route("api/query")]
public class BigQueryController : ControllerBase
{
    private readonly BigQueryService _bigQueryService;
    private readonly TraceIdProvider _traceProvider;

    public BigQueryController(BigQueryService service, TraceIdProvider traceIdProvider)
    {
        _bigQueryService = service;
        _traceProvider = traceIdProvider;
    }

    [HttpGet("transactionsByMonth")]
    public async Task<IActionResult> GetAllTransactionsByMonthAsync([FromQuery] int? month, [FromQuery] int? year)
    {
        List<TransactionsListByMonthDto> result = await _bigQueryService.GetAllTransactionsAsync(month, year);
        return Ok(new ApiResponse<List<TransactionsListByMonthDto>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Result = result,
            TraceId = _traceProvider.TraceId
        });
    }
}