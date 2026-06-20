using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Exceptions;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using BudgetTracker.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Controllers;

[ApiController]
[Route("api/banks")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = SharedConstants.Jwt.Policies.DOWNSTREAM_POLICY)]
public class BankController : ControllerBase
{
    private readonly BankService _bankService;
    private readonly TraceIdProvider _traceProvider;

    public BankController(BankService bankService, TraceIdProvider trace)
    {
        _bankService = bankService;
        _traceProvider = trace;
    }

    /// <summary>
    /// Inserts a new Bank
    /// </summary>
    /// <param name="payload">The Bank name that should be inserted</param>
    /// <returns>Status of the inserted bank</returns>
    /// <response code="201">Returns the status message</response>
    /// <response code="400">
    /// Returns when Payload is invalid like name contains invalid characters
    /// </response>
    /// <response code="500">
    /// Returns when any internal exception or DB updates failed due to constraints violations.
    /// </response>
    /// <exception cref="InvalidPayloadException">
    /// Thrown when <paramref name="payload"/> fails validation (e.g., invalid characters).
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// Thrown if the bank cannot be inserted in the database, due to any constraint violations
    /// </exception>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse>> InsertAsync([FromBody] InsertBankDto payload)
    {
        await _bankService.InsertBankAsync(payload);
        return Ok(new ApiResponse
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

        return Ok(new ApiResponse
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Message = "Bank updated successfully"
        });
    }
}