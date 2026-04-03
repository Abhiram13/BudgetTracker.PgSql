using System.Globalization;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public TransactionRepository(WriteDbContext write, ReadDbContext read)
    {
        _writeDbContext = write;
        _readDbContext = read;
    }

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

    public async Task<Transaction> InsertOneTransactionAsync(Transaction payload)
    {
        await _writeDbContext.Transactions.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }
    
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

    public async Task<int> CountOfAllTransactionsAsync(int? month, int? year) // TODO: Move validations to Transactions Service class
    {
        int m = month ?? DateTime.Now.Month;
        int y = year ?? DateTime.Now.Year;

        if (m > DateTime.Now.Month)
        {
            throw new InvalidPayloadException("Month cannot be greater than current month.");
        }

        if (y > DateTime.Now.Year)
        {
            throw new InvalidPayloadException("Year cannot be greater than current year.");
        }
        
        DateOnly start = new DateOnly(y, m, 1);
        DateOnly end = start.AddMonths(1);

        int count = await _readDbContext.Transactions
            .Where(t => t.Date >= start && t.Date < end)
            .CountAsync();
        
        return count;
    }

    public async Task UpdateTransactionAsync(UpdateTransactionDto payload, int id)
    {
        Transaction? tx = await _writeDbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (tx == null) throw new BadHttpRequestException("Transaction not found");
        
        tx.ActualAmount = payload.ActualAmount;
        tx.Description = payload.Description;
        tx.Amount = payload.Amount;
        tx.Date = payload.Date;
        tx.CategoryId = payload.CategoryId;
        tx.FromBank = payload.FromBank;
        tx.ToBank = payload.ToBank;
        tx.Type = payload.Type;
        tx.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task<List<DateOnly>> GetGroupOfDatesAsync()
    {
        List<DateOnly> dates = await _readDbContext.Transactions.GroupBy(t => t.Date).Select(t => t.Key).ToListAsync();

        return dates;
    }
}