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

    public async Task<TransactionByDateDto> GetAllTransactionsByDateAsync(string transactionDate)
    {
        DateOnly date = DateOnly.Parse(transactionDate);
        TransactionByDateDto? result = await _writeDbContext.Transactions
            .Where(t => t.Date == date)
            .GroupBy(t => t.Date)
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
    
    public async Task<TransactionsByMonthDto?> GetDebitCreditByDateAsync(DateOnly transactionDate)
    {
        TransactionsByMonthDto? result = await _writeDbContext.Transactions
            .Where(t => t.Date == transactionDate)
            .GroupBy(t => t.Date)
            .Select(t => new TransactionsByMonthDto
            {
                Credit = t.Where(d => d.Type == TransactionType.Credit).Sum(c => c.ActualAmount),
                Debit = t.Where(d => d.Type == TransactionType.Debit).Sum(c => c.ActualAmount),
                Date = transactionDate
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<CategoryTransactionsSumDto> GetTransactionsSumsByCategoryAsync()
    {
        throw new NotImplementedException();
    }
}