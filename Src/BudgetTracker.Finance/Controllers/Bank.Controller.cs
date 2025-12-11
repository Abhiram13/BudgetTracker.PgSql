using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Finance.Api.Controllers;

[ApiController]
[Route("api/banks")]
public class BankController : ControllerBase
{
    private readonly BankService _bankService;
    private readonly TraceIdProvider _traceProvider;

    public BankController(BankService bankService, TraceIdProvider trace)
    {
        _bankService = bankService;
        _traceProvider = trace;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> InsertAsync([FromBody] InsertBankDto payload)
    {
        DateTime now = DateTime.UtcNow;
        Bank bank = new Bank
        {
            CreatedAt = now,
            UpdatedAt = now,
            Name = payload.Name
        };

        await _bankService.InsertBankAsync(bank);
        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId = _traceProvider.TraceId,
            Message = "Bank created successfully"
        });
    }
}