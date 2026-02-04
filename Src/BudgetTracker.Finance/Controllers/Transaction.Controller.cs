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
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        Transaction transaction = new Transaction
        {
            CreatedAt = today,
            UpdatedAt = today,
            ActualAmount = payload.ActualAmount,
            Amount = payload.Amount,
            Description = payload.Description,
            CategoryId = payload.CategoryId,
            Date = payload.Date,
            FromBank = payload.FromBank,
            ToBank = payload.ToBank,
            Type = payload.Type,
            DueId = payload.DueId,
            EMIId = payload.EmiId
        };

        await _transactionService.InsertTransactionAsync(transaction);
        return Ok(new ApiResponse<string>
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
}