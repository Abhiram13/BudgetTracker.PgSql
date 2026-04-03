using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> InsertOneTransactionAsync(Transaction payload);
    Task<TransactionByDateDto> GetAllTransactionsByDateAsync(string transactionDate);

    /// <summary>
    /// Get total debit and total credit on a transaction date to be added in Big Query. <b>NOT TO BE USED DIRECTLY IN APIs</b>
    /// </summary>
    /// <param name="transactionDate"></param>
    /// <returns></returns>
    Task<TransactionCreditDebitByDateDto?> GetDebitCreditByDateAsync(DateOnly transactionDate);
    Task<int> CountOfAllTransactionsAsync(int?  month, int? year);
    Task UpdateTransactionAsync(UpdateTransactionDto payload, int id);
    Task<List<DateOnly>> GetGroupOfDatesAsync();
}