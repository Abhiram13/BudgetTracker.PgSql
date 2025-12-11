using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly WriteDbContext _writeDbContext;

    public TransactionRepository(WriteDbContext write)
    {
        _writeDbContext = write;
    }

    public async Task<List<TransactionListDto>> GetTransactionsAsync(int? month = null, int? year = null)
    {
        IQueryable<Transaction> query = _writeDbContext.Transactions.AsQueryable();

        if (month is not null)
        {
            query = query.Where(q => q.Date.Month == month);
        }

        if (year is not null)
        {
            query = query.Where(q => q.Date.Year == year);
        }

        if (month is not null && year is not null)
        {
            query = query.Where(q => q.Date.Month == month && q.Date.Year == year);
        }

        List<TransactionListDto> list = await query
            .GroupBy(t => t.Date.Date)
            .Select(g => new TransactionListDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Debit = g.Where(t => t.Type == TransactionType.Debit).Sum(t => t.ActualAmount),
                Credit = g.Where(t => t.Type == TransactionType.Credit).Sum(t => t.ActualAmount),
            })
            .ToListAsync();

        return list;
    }

    public async Task<TransactionByDateDto> GetTransactionsByDateAsync(string transactionDate)
    {
        DateTime date = DateTime.Parse(transactionDate);
        TransactionByDateDto? result = await _writeDbContext.Transactions
            .Where(t => t.Date.Date == date.Date)
            .GroupBy(t => t.Date.Date)
            .Select(t => new TransactionByDateDto
            {
                Debit = t.Where(d => d.Type == TransactionType.Debit).Sum(d => d.ActualAmount),
                Credit = t.Where(c => c.Type == TransactionType.Credit).Sum(c => c.ActualAmount),
                TransactionsList = t.Select(l => new TransactionByDateDto.Transactions
                {
                    Amount = l.ActualAmount,
                    Type = l.Type
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
}