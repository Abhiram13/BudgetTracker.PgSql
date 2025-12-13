using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> InsertOneTransactionAsync(Transaction payload);
    Task<List<TransactionListDto>> GetTransactionsAsync(int? month = null, int? year = null);
    Task<TransactionByDateDto> GetAllTransactionsByDateAsync(string transactionDate);
    Task<TransactionListDto?> GetDebitCreditByDateAsync(string transactionDate);
}