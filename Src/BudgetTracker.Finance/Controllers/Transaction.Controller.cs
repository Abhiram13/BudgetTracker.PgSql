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
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;
    private readonly TraceIdProvider _traceProvider;

    public TransactionController(TransactionService transactionService, TraceIdProvider trace)
    {
        _transactionService = transactionService;
        _traceProvider = trace;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> InsertAsync([FromBody] InsertTransactionDto payload)
    {
        await _transactionService.InsertTransactionAsync(payload);
        return StatusCode(201, new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId = _traceProvider.TraceId,
            Message = "Transaction created successfully"
        });
    }

    [HttpGet("date/{date}")]
    public async Task<ActionResult<ApiResponse<TransactionByDateDto>>> GetTransactionsByDateAsync([FromRoute] string date)
    {
        TransactionByDateDto result = await _transactionService.GetTransactionsByDateAsync(date);
        return Ok(new ApiResponse<TransactionByDateDto>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = result
        });
    }

    [HttpGet("count")]
    public async Task<IActionResult> CountOfTransactionsAsync([FromQuery] int? month, [FromQuery] int? year)
    {
        int count = await _transactionService.CountOfAllTransactionsAsync(month, year);
        
        Console.WriteLine(count);

        return Ok(new ApiResponse<int>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = count
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransactionAsync([FromRoute] int id, [FromBody] UpdateTransactionDto payload)
    {
        await _transactionService.UpdateTransactionAsync(payload, id);
        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Message = "Transaction updated successfully"
        });
    }

    // [HttpGet("bigQuery")]
    // public async Task<IActionResult> BigQueryUpdatesAsync()
    // {
    //     await _transactionService.BigQueryUpdatesAsync();
    //     
    //     return Ok(new  ApiResponse<string>
    //     {
    //         StatusCode = System.Net.HttpStatusCode.OK,
    //         TraceId = _traceProvider.TraceId,
    //         Message = "Big query updated successfully"
    //     });
    // }
}