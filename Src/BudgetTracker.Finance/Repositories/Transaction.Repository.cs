using System.Globalization;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Repository;

/// <summary>
/// Performs the DB operations on Write and Read replicas on <see cref="Transaction"/> table
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    /// <summary>
    /// Peforms the DB operations on Write and Read replicas on <see cref="Transaction"/> table
    /// </summary>
    /// <param name="write"><see cref="WriteDbContext"/> injection used to perform write DB operations</param>
    /// <param name="read"><see cref="ReadDbContext"/> injection used to perform read DB operations</param>
    public TransactionRepository(WriteDbContext write, ReadDbContext read)
    {
        _writeDbContext = write;
        _readDbContext = read;
    }

    /// <inheritdoc />
    public async Task<TransactionByDateDto> GetAllTransactionsByDateAsync(string transactionDate)
    {
        if (!DateOnly.TryParseExact(transactionDate, "yyyy-MM-dd", out DateOnly _))
        {
            throw new InvalidDateException();
        }
        
        DateOnly date = DateOnly.Parse(transactionDate, CultureInfo.InvariantCulture);
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (date > today)
        {
            throw new InvalidDateException();
        }
        
        TransactionByDateDto? result = await _readDbContext.Transactions
            .Where(t => t.Date == date)
            .GroupBy(t => t.Date)
            .Select(t => new TransactionByDateDto
            {
                Debit = t.Where(d => d.Type == TransactionType.Debit).Sum(d => d.Amount),
                Credit = t.Where(c => c.Type == TransactionType.Credit).Sum(c => c.Amount),
                TransactionsList = t.Select(l => new TransactionByDateDto.Transactions
                {
                    Amount = l.Amount,
                    Type = l.Type,
                    Description = l.Description,
                    TransactionId = l.Id
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return result ?? new TransactionByDateDto();
    }

    /// <inheritdoc />
    public async Task<Transaction> InsertOneTransactionAsync(Transaction payload)
    {
        await _writeDbContext.Transactions.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }
    
    /// <inheritdoc />
    public async Task<TransactionCreditDebitByDateDto?> GetDebitCreditByDateAsync(DateOnly transactionDate)
    {
        TransactionCreditDebitByDateDto? result = await _readDbContext.Transactions
            .Where(t => t.Date == transactionDate)
            .GroupBy(t => t.Date)
            .Select(t => new TransactionCreditDebitByDateDto
            {
                Credit = t.Where(d => d.Type == TransactionType.Credit).Sum(c => c.Amount),
                Debit = t.Where(d => d.Type == TransactionType.Debit).Sum(c => c.Amount),
                Date = transactionDate,
                Count = t.Count()
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<int> CountOfAllTransactionsAsync(int? month, int? year) // TODO: Move validations to Transactions Service class
    {
        (DateOnly start, DateOnly end) = ValidateMonthYear(month, year);
        int count = await _readDbContext.Transactions
            .Where(t => t.Date >= start && t.Date < end)
            .CountAsync();
        
        return count;
    }

    /// <inheritdoc />
    public async Task UpdateTransactionAsync(UpdateTransactionDto payload, int id)
    {
        Transaction? tx = await _writeDbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (tx == null) throw new BadHttpRequestException("Transaction not found"); // TODO: Should use 'InvalidPayloadException'?
        
        tx.Update(
            actualAmount: payload.ActualAmount,
            amount: payload.Amount,
            description: payload.Description,
            date: payload.Date,
            type: payload.Type,
            categoryId: payload.CategoryId,
            fromBank: payload.FromBank,
            toBank: payload.ToBank
        );
        
        await _writeDbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<List<DateOnly>> GetGroupOfDatesAsync()
    {
        List<DateOnly> dates = await _readDbContext.Transactions.GroupBy(t => t.Date).Select(t => t.Key).ToListAsync();

        return dates;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TransactionsListByMonthYear>> GetListOfTransactionsByMonthYear(int? month, int? year)
    {
        (DateOnly start, DateOnly end) = ValidateMonthYear(month, year);
        List<TransactionsListByMonthYear> result = await _readDbContext.Transactions 
            .Where(t => t.Date >= start && t.Date < end)
            .GroupBy(t => t.Date)
            .OrderBy(o => o.Key)
            .Select(t => new TransactionsListByMonthYear
            {
                TransactionDate = t.Key,
                Debit = t.Where(d => d.Type == TransactionType.Debit).Sum(d => d.Amount),
                Credit = t.Where(c => c.Type == TransactionType.Credit).Sum(c => c.Amount),
            })
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryBankTransactionsByMonthYear>> GetListOfCategoryTransactionsByMonthYear(int? month, int? year)
    {
        (DateOnly start, DateOnly end) = ValidateMonthYear(month, year);
        List<CategoryBankTransactionsByMonthYear> result = await _readDbContext.Transactions
            .Where(t => t.Date >= start && t.Date < end)
            .GroupBy(t => new { t.CategoryId, t.CategoryF.Name })
            .Select(g => new CategoryBankTransactionsByMonthYear
            {
                Name = g.Key.Name,
                Amount = g.Where(d => d.Type == TransactionType.Debit).Sum(d => d.Amount),
            })
            .ToListAsync();;

        return result;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryBankTransactionsByMonthYear>> GetListOfBankTransactionsByMonthYear(int? month, int? year)
    {
        (DateOnly start, DateOnly end) = ValidateMonthYear(month, year);
        List<CategoryBankTransactionsByMonthYear> result = await _readDbContext.Transactions
            .Where(t => t.Date >= start && t.Date < end)
            .GroupBy(t => new { t.FromBank, t.FromBankF.Name })
            .Select(g => new CategoryBankTransactionsByMonthYear
            {
                Name = g.Key.Name,
                Amount = g.Where(d => d.Type == TransactionType.Debit).Sum(d => d.Amount),
            })
            .ToListAsync();;

        return result;
    }

    private (DateOnly start, DateOnly end) ValidateMonthYear(int? month, int? year)
    {
        int m = month ?? DateTime.UtcNow.Month;
        int y = year ?? DateTime.UtcNow.Year;

        // BUG: If month is above current month and year is less than current year. It is a valid case. But even then, below if condition will throw error.
        if (m > DateTime.UtcNow.Month)
        {
            throw new InvalidPayloadException("Month cannot be greater than current month.");
        }

        if (y > DateTime.UtcNow.Year)
        {
            throw new InvalidPayloadException("Year cannot be greater than current year.");
        }
        
        DateOnly start = new DateOnly(y, m, 1);
        DateOnly end = start.AddMonths(1);
        
        return (start, end);
    }
}