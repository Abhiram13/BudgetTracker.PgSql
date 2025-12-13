using BudgetTracker.Shared.Models;
using BudgetTracker.Warehouse.Models;
using BudgetTracker.Warehouse.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Warehouse.Controllers;

[ApiController]
[Route("api/query")]
public class BigQueryController : ControllerBase
{
    private readonly BigQueryService _bigQueryService;

    public BigQueryController(BigQueryService service)
    {
        _bigQueryService = service;
    }

    [HttpPost("transactionsByDate")]
    public async Task<IActionResult> InsertTransactionsByDateAsync([FromBody] DateTransactionsDto payload)
    {
        await _bigQueryService.InsertTransactionByDateAsync(payload);
        return StatusCode(201, new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            Message = "Transactions by date added in Big Query",
            TraceId = ""
        });
    }
}