using System.Diagnostics;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Services;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Finance.Mvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace BudgetTracker.Finance.Mvc.Controllers;

[AllowAnonymous]
[Route("view/transactions")]
public class TransactionViewController : Controller
{
    private readonly TransactionService _transactionService;
    private readonly ILogger<TransactionViewController> _logger;
    
    public TransactionViewController(TransactionService transactionService, ILogger<TransactionViewController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int? month, int? year)
    {
        int queryMonth = month ?? DateTime.UtcNow.Month;
        int queryYear = year ?? DateTime.UtcNow.Year;

        _logger.LogInformation("Month: {Month} and Year: {Year}", month, year);
        
        int countTask = await _transactionService.CountOfAllTransactionsAsync(queryMonth, queryYear);
        IReadOnlyList<TransactionsListByMonthYear> listTask = await _transactionService.GetTransactionsByMonthYearAsync(queryMonth, queryYear);        
        
        TransactionModel model = new TransactionModel
        {
            Count = countTask,
            Transactions = listTask,
            SelectedMonth = queryMonth,
            SelectedYear = queryYear
        };
        
        return View("~/Views/Home/Index.cshtml", model);
    }

    public IActionResult PreviousMonth(int month, int year)
    {
        DateTime date = new DateTime(year, month, 1).AddMonths(-1);
        
        _logger.LogInformation(date.ToString());
        
        return RedirectToAction(nameof(Index), new { month = date.Month, year = date.Year });
    }
}