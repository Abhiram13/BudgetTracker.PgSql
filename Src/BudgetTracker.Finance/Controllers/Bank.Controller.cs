using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using BudgetTracker.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BudgetTracker.Finance.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = SharedConstants.Jwt.Policies.DOWNSTREAM_POLICY)]
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
        await _bankService.InsertBankAsync(payload);
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
        await _bankService.UpdateBankAsync(payload, id);

        return Ok(new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Message = "Bank updated successfully"
        });
    }
}