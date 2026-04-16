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
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        Bank bank = new Bank
        {
            CreatedAt = today,
            UpdatedAt = today,
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
    public async Task<ActionResult<ApiResponse<List<BankDto>>>> GetAllBanksAsync()
    {
        List<BankDto> list = await _bankService.GetBankListsAsync();
        return Ok(new ApiResponse<List<BankDto>>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = list
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBankByIdAsync([FromRoute] int id)
    {
        Bank bank = await _bankService.GetBankByIdAsync(id);

        return Ok(new ApiResponse<BankDto>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = new BankDto
            {
                Id = bank.Id,
                Name = bank.Name
            }
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOneAsync([FromRoute] int id, [FromBody] InsertBankDto payload)
    {
        await _bankService.UpdateBankAsync(new Bank { Name = payload.Name, Id = id });

        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Message = "Bank updated successfully"
        });
    }
}