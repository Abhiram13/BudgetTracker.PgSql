using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Warehouse.Models;
using BudgetTracker.Warehouse.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Warehouse.Controllers;

[ApiController]
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

    [HttpPost("transactionsByDate")]
    public async Task<IActionResult> InsertTransactionsByDateAsync([FromBody] DateTransactionsDto payload)
    {
        await _bigQueryService.InsertTransactionByDateAsync(payload);
        return StatusCode(201, new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            Message = "Transactions by date added in Big Query",
            TraceId = _traceProvider.TraceId
        });
    }

    [HttpGet("transactionsByDate")]
    public async Task<IActionResult> GetAllTransactionsAsync([FromQuery] int? month, [FromQuery] int? year)
    {
        List<DateTransactionsDto> result = await _bigQueryService.GetAllTransactionsAsync(month, year);
        return Ok(new ApiResponse<List<DateTransactionsDto>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Result = result,
            TraceId = _traceProvider.TraceId
        });
    }
}