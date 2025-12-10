using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Finance.Api.Controllers;

[ApiController]
[Route("bank")]
public class BankController : ControllerBase
{
    private readonly BankService _bankService;

    public BankController(BankService bankService)
    {
        _bankService = bankService;
    }

    [HttpPost]
    public async Task<IActionResult> InsertAsync([FromBody] InsertBankDto payload)
    {
        DateTime now = DateTime.UtcNow;
        Bank bank = new Bank
        {
            CreatedAt = now,
            UpdatedAt = now,
            Name = payload.Name
        };

        await _bankService.InsertBankAsync(bank);
        return Ok();
    }
}