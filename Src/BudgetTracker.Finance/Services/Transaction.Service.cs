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
    private readonly TransactionsMetaService _transactionsMetaService;

    public TransactionService(ITransactionRepository repository, PublisherService publisherService, ILogger<TransactionService> logger, TransactionsMetaService transactionsMetaService)
    {
        _repository = repository;
        _publisher = publisherService;
        _logger = logger;
        _transactionsMetaService = transactionsMetaService;
    }

    public async Task InsertTransactionAsync(InsertTransactionDto payload)
    {
        if (payload.FromBank is null && payload.ToBank is null)
        {
            throw new BadHttpRequestException("Invalid payload provided");
        }
        
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        Transaction transaction = new Transaction
        {
            CreatedAt = today,
            UpdatedAt = today,
            ActualAmount = payload.ActualAmount,
            Amount = payload.Amount,
            Description = payload.Description,
            CategoryId = payload.CategoryId,
            Date = payload.Date,
            FromBank = payload.FromBank,
            ToBank = payload.ToBank,
            Type = payload.Type,
        };

        await _repository.InsertOneTransactionAsync(transaction);

        if (payload.DueId is not null || payload.EmiId is not null || !string.IsNullOrEmpty(payload.Tags))
        {
            TransactionsMeta meta = new TransactionsMeta
            {
                TransactionId = transaction.Id,
                DueId = payload.DueId,
                EmiId = payload.EmiId,
                Tags = payload.Tags,
                CreatedAt = today,
                UpdatedAt = today,
            };

            await _transactionsMetaService.InsertTransactionMetaAsync(meta);
        }
        
        await UpdateTransactionsByMonthAsync(payload.Date);
    }

    public async Task<TransactionByDateDto> GetTransactionsByDateAsync(string transactionDate)
    {
        return await _repository.GetAllTransactionsByDateAsync(transactionDate);
    }

    public async Task<int> CountOfAllTransactionsAsync(int? month, int? year)
    {
        return await _repository.CountOfAllTransactionsAsync(month, year);
    }

    public async Task UpdateTransactionAsync(UpdateTransactionDto payload, int id)
    {
        await _repository.UpdateTransactionAsync(payload, id);

        if (payload.DueId is not null || payload.EmiId is not null || !string.IsNullOrEmpty(payload.Tags))
        {
            UpdateTransactionMetaDto metaDto = new UpdateTransactionMetaDto
            {
                DueId = payload.DueId,
                EmiId = payload.EmiId,
                Tags = payload.Tags,
            };
            await _transactionsMetaService.UpdateTransactionMetaAsync(metaDto, transactionId: id);
        }

        await UpdateTransactionsByMonthAsync(payload.Date);
    }
    
    private async Task UpdateTransactionsByMonthAsync(DateOnly date)
    {
        TransactionsListByMonthDto? result = null;
        
        try
        {
            result = await _repository.GetDebitCreditByDateAsync(date);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception at Transaction Service insert. Message = {0}", e.Message);
        }

        if (result is not null)
        {
            string message = JsonSerializer.Serialize(result!);
            
            // TODO: Get Trace ID here
            await _publisher.PublishMessageAsync(requestMessage: message, eventType: PubSubFinanceEvents.DATEWISE_TRANSACTIONS_LIST, traceId: null);
        }   
    }
}