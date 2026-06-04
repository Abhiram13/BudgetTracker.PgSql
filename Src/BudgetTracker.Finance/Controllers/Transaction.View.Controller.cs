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
    
    public TransactionViewController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    // [HttpGet]
    public async Task<IActionResult> Index()
    {
        int month = DateTime.UtcNow.AddMonths(-1).Month;
        int year = DateTime.UtcNow.Year;
        int countTask = await _transactionService.CountOfAllTransactionsAsync(month, year);
        IReadOnlyList<TransactionsListByMonthYear> listTask = await _transactionService.GetTransactionsByMonthYearAsync(month, year);
        
        TransactionModel model = new TransactionModel
        {
            Count = countTask,
            Transactions = listTask
        };
        
        return View("~/Views/Home/Index.cshtml", model);
    }
}