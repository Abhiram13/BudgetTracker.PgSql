using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> InsertOneTransactionAsync(Transaction payload);
    Task<List<TransactionListDto<string>>> GetTransactionsAsync(int? month = null, int? year = null);
    Task<TransactionByDateDto> GetAllTransactionsByDateAsync(string transactionDate);

    /// <summary>
    /// Get total debit and total credit on a transaction date to be added in Big Query. <b>NOT TO BE USED DIRECTLY IN APIs</b>
    /// </summary>
    /// <param name="transactionDate"></param>
    /// <returns></returns>
    Task<TransactionListDto<DateTime>?> GetDebitCreditByDateAsync(DateTime transactionDate);
}