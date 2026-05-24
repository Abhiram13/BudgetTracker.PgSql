using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Interfaces;

/// <summary>
/// Repository that defines database access contract for managing <see cref="Transaction"/> entity.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Inserts a single transaction record into the database.
    /// </summary>
    /// <param name="payload">The <see cref="Transaction"/> entity to insert.</param>
    /// <returns>The inserted <see cref="Transaction"/> entity, along with its generated database ID.</returns>
    /// <exception cref="DbUpdateException">Thrown if a any database constraint is violated.</exception>
    Task<Transaction> InsertOneTransactionAsync(Transaction payload);
    
    /// <summary>
    /// Retrieves all transactions recorded on a specific date. Can return empty <see cref="TransactionByDateDto"/> if none found.
    /// </summary>
    /// <param name="transactionDate">The Date in format <c>yyyy-MM-dd</c> to fetch recorded transactions on that date</param>
    /// <returns><see cref="TransactionByDateDto"/> Contains collection of transactions with the specified date.</returns>
    /// <exception cref="InvalidDateException">Thrown when given date is not in <c>yyyy-MM-dd</c> format or if future date is given</exception>
    Task<TransactionByDateDto> GetAllTransactionsByDateAsync(string transactionDate);

    /// <summary>
    /// Calculates total debit, total credit and total count of transactions in a given date.
    /// </summary>
    /// <remarks>
    /// <b>This method is private, not an API and should be used only in or through <see cref="TransactionService"/></b>. <br />
    /// This method is only used to add analytics data by transaction date in BigQuery. This happens after a transaction is inserted successfully
    /// </remarks>
    /// <param name="transactionDate">The specific <see cref="DateOnly"/> to filter.</param>
    /// <returns><see cref="TransactionCreditDebitByDateDto"/> with debit, credit and count.</returns>
    Task<TransactionCreditDebitByDateDto?> GetDebitCreditByDateAsync(DateOnly transactionDate);
    
    /// <summary>
    /// Counts total transactions for given month and year
    /// </summary>
    /// <param name="month">Optional month (1-12) to filter the count. Defaults to current month</param>
    /// <param name="year">Optional year to filter the count. Defaults to current year</param>
    /// <returns><see cref="Int64"/> - The total count of transactions matching the filter.</returns>
    /// <exception cref="InvalidPayloadException">Thrown when given month or year is greater than current month and year</exception>
    Task<int> CountOfAllTransactionsAsync(int? month, int? year);
    
    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <remarks>All the values provided will be replaced</remarks>
    /// <param name="payload">The <see cref="UpdateTransactionDto"/> containing updated values</param>
    /// <param name="id">The ID of the transaction to update.</param>
    /// <exception cref="BadHttpRequestException">Thrown if no transaction exists with the provided <paramref name="id"/>.</exception>
    Task UpdateTransactionAsync(UpdateTransactionDto payload, int id);
    
    /// <summary>
    /// Retrieves a distinct list of all dates that have recorded transactions.
    /// </summary>
    /// <remarks>
    /// <b>This method is Private and should not be used in API. This is used to fetch all dates so that big query can be re-updated in bulk.</b>
    /// </remarks>
    /// <returns>A list of <see cref="DateOnly"/> values of all transactions</returns>
    [Obsolete(message: "Big query is not being used. So this method will be removed in future.")]
    Task<List<DateOnly>> GetGroupOfDatesAsync();
    
    /// <summary>
    /// Retrieves list of monthly based transactions by given month and year.
    /// </summary>
    /// <returns>
    /// List of <see cref="TransactionsListByMonthYear"/>
    /// </returns>
    /// <exception cref="InvalidPayloadException">Thrown if month or year are invalid or contains future values.</exception>
    Task<IReadOnlyList<TransactionsListByMonthYear>> GetListOfTransactionsByMonthYear(int? month, int? year);
    
    /// <summary>
    /// Retrieves list of all transactions grouped by all Categories by given month and date
    /// </summary>
    /// <exception cref="InvalidPayloadException">Thrown if month or year are invalid or contains future values.</exception>
    Task<IReadOnlyList<CategoryBankTransactionsByMonthYear>> GetListOfCategoryTransactionsByMonthYear(int? month, int? year);
    
    /// <summary>
    /// Retrieves list of all transactions grouped by all Banks by given month and date
    /// </summary>
    /// <exception cref="InvalidPayloadException">Thrown if month or year are invalid or contains future values.</exception>
    Task<IReadOnlyList<CategoryBankTransactionsByMonthYear>> GetListOfBankTransactionsByMonthYear(int? month, int? year);
}