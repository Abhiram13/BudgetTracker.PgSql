using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Repository;

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

        // If no transaction meta found, create a new one.
        if (meta is null)
        {
            meta = TransactionsMeta.Create(transactionId: transactionId);
            await _dbContext.TransactionsMeta.AddAsync(meta);
        }
        
        // else update other fields since transaction id will present.
        meta.Update(dueId: payload.DueId, emiId: payload.EmiId, tags: payload.Tags);
        await _dbContext.SaveChangesAsync();
    }
}