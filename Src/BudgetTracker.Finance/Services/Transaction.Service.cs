using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using System.Text.Json;

namespace BudgetTracker.Finance.Services;

public class TransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly PublisherService _publisher;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository repository, PublisherService publisherService, ILogger<TransactionService> logger)
    {
        _repository = repository;
        _publisher = publisherService;
        _logger = logger;
    }

    public async Task<Transaction> InsertTransactionAsync(Transaction payload)
    {
        if (payload.FromBank is null && payload.ToBank is null)
        {
            throw new BadHttpRequestException("Invalid payload provided");
        }

        Transaction transaction = await _repository.InsertOneTransactionAsync(payload);
        TransactionsListByMonthDto? result = null;

        try
        {
            result = await _repository.GetDebitCreditByDateAsync(payload.Date);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception at Transaction Service insert. Message = {0}", e.Message);
        }

        if (result is not null)
        {
            string message = JsonSerializer.Serialize(result!);
            await _publisher.PublishMessageAsync(requestMessage: message, eventType: PubSubFinanceEvents.DATEWISE_TRANSACTIONS_LIST, traceId: null);
        }

        return transaction;
    }

    public async Task<TransactionByDateDto> GetTransactionsByDateAsync(string transactionDate)
    {
        return await _repository.GetAllTransactionsByDateAsync(transactionDate);
    }
}