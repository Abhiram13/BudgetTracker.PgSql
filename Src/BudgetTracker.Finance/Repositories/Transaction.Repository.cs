using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Models;

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
                Debit = t.Where(d => d.Type == TransactionType.Debit).Sum(d => d.ActualAmount) ?? 0,
                Credit = t.Where(c => c.Type == TransactionType.Credit).Sum(c => c.ActualAmount) ?? 0,
                TransactionsList = t.Select(l => new TransactionByDateDto.Transactions
                {
                    Amount = l.Amount,
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
    
    public async Task<TransactionsListByMonthDto?> GetDebitCreditByDateAsync(DateOnly transactionDate)
    {
        TransactionsListByMonthDto? result = await _writeDbContext.Transactions
            .Where(t => t.Date == transactionDate)
            .GroupBy(t => t.Date)
            .Select(t => new TransactionsListByMonthDto
            {
                Credit = t.Where(d => d.Type == TransactionType.Credit).Sum(c => c.Amount),
                Debit = t.Where(d => d.Type == TransactionType.Debit).Sum(c => c.Amount),
                Date = transactionDate,
                Count = t.Count()
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<CategoryTransactionsSumDto> GetTransactionsSumsByCategoryAsync()
    {
        throw new NotImplementedException();
    }
}