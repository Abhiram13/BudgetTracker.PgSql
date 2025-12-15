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

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<BankListDto>>>> GetAllBanksAsync()
    {
        List<BankListDto> list = await _bankService.GetBankListsAsync();
        return Ok(new ApiResponse<List<BankListDto>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = list
        });
    }
}