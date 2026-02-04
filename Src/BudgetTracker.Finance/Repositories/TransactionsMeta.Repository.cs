using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Repositories;

public class TransactionsMetaRepository : ITransactionsMetaRepository
{
    private readonly WriteDbContext _dbContext;

    public TransactionsMetaRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task InsertMetaAsync(TransactionsMeta payload)
    {
        await _dbContext.AddAsync(payload);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMetaAsync(UpdateTransactionMetaDto payload, int transactionId)
    {
        TransactionsMeta? meta = await _dbContext.TransactionsMeta.FirstOrDefaultAsync(m => m.TransactionId == transactionId); //TODO: Check if transaction by ID exists as well.
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);

        if (meta is null)
        {
            meta = new TransactionsMeta
            {
                TransactionId = transactionId,
                CreatedAt = date,
            };
            
            await _dbContext.TransactionsMeta.AddAsync(meta);
        }

        meta.EmiId = payload.EmiId;
        meta.DueId = payload.DueId;
        meta.Tags = payload.Tags;
        meta.UpdatedAt = date;
        
        await _dbContext.SaveChangesAsync();
    }
}