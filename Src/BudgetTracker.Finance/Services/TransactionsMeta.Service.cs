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

    public async Task InsertTransactionMetaAsync(InsertTransactionMetaDto payload)
    {
        if (ShouldTransactionMetaCreated(payload.DueId, payload.EmiId, payload.Tags))
        {
            TransactionsMeta data = TransactionsMeta.Create(
                transactionId: payload.TransactionId,
                emiId: payload.EmiId,
                tags: payload.Tags,
                dueId: payload.DueId
            );
            
            await _repository.InsertMetaAsync(data);
            _logger.LogInformation("Transaction meta data with Transaction-Id = {TransactionId} has been inserted successfully", payload.TransactionId);
        }
    }

    public async Task UpdateTransactionMetaAsync(UpdateTransactionMetaDto payload, int transactionId)
    {
        await _repository.UpdateMetaAsync(payload, transactionId);
    }

    private bool ShouldTransactionMetaCreated(int? dueId, int? emiId, string? tags)
    {
        return dueId is not null || emiId is not null || !string.IsNullOrEmpty(tags);
    }
}