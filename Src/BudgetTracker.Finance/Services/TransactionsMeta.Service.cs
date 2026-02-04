using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Services;

public class TransactionsMetaService
{
    private readonly ITransactionsMetaRepository  _repository;
    private readonly ILogger<TransactionsMetaService> _logger;

    public TransactionsMetaService(ITransactionsMetaRepository repository, ILogger<TransactionsMetaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task InsertTransactionMetaAsync(TransactionsMeta payload)
    {
        // DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
        // TransactionsMeta data = new TransactionsMeta
        // {
        //     EmiId = payload.EmiId,
        //     DueId = payload.DueId,
        //     TransactionId = payload.TransactionId,
        //     Tags = payload.Tags,
        //     CreatedAt = date,
        //     UpdatedAt = date,
        // };

        await _repository.InsertMetaAsync(payload);
    }

    public async Task UpdateTransactionMetaAsync(UpdateTransactionMetaDto payload, int transactionId)
    {
        await _repository.UpdateMetaAsync(payload, transactionId);
    }
}