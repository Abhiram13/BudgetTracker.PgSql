using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Finance.Controllers;

[ApiController]
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
        DateTime now = DateTime.UtcNow;
        Transaction transaction = new Transaction
        {
            CreatedAt = now,
            UpdatedAt = now,
            ActualAmount = payload.ActualAmount,
            Amount = payload.Amount,
            Description = payload.Description,
            CategoryId = payload.CategoryId,
            Date = DateTime.Parse(payload.Date),
            FromBank = payload.FromBank,
            ToBank = payload.ToBank,
            Type = payload.Type
        };

        await _transactionService.InsertTransactionAsync(transaction);
        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId = _traceProvider.TraceId,
            Message = "Transaction created successfully"
        });
    }

    // [HttpGet]
    // public async Task<ActionResult<ApiResponse<List<TransactionListDto<string>>>>> GetTransactionsAsync([FromQuery] int? month, [FromQuery] int? year)
    // {
    //     List<TransactionListDto<string>> list = await _transactionService.GetAllTransactionsAsync(month, year);
    //     return Ok(new ApiResponse<List<TransactionListDto<string>>>
    //     {
    //         StatusCode = System.Net.HttpStatusCode.OK,
    //         TraceId = _traceProvider.TraceId,
    //         Result = list
    //     });
    // }

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
}