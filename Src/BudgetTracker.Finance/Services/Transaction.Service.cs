using System.Diagnostics;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using System.Text.Json;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Shared.Exceptions;

namespace BudgetTracker.Finance.Services;

public class TransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly PublisherService _publisher;
    private readonly ILogger<TransactionService> _logger;
    private readonly TransactionsMetaService _transactionsMetaService;
    private static readonly ActivitySource _activitySource = new ActivitySource(nameof(TransactionService));

    public TransactionService(ITransactionRepository repository, PublisherService publisherService, ILogger<TransactionService> logger, TransactionsMetaService transactionsMetaService)
    {
        _repository = repository;
        _publisher = publisherService;
        _logger = logger;
        _transactionsMetaService = transactionsMetaService;
    }

    private void InsertValidations(TransactionDto payload)
    {
        bool IsNotValidBanks() => payload.FromBank is 0 || payload.ToBank is 0 || (payload.FromBank is null && payload.ToBank is null);
        
        bool IsNotValidAmount() => payload.Amount is 0 || payload.Amount < 0 || payload.ActualAmount < 0;
        
        bool IsNotValidDescription() => string.IsNullOrEmpty(payload.Description);
        
        bool IsNotValidCategoryId() => payload.CategoryId is 0 || payload.CategoryId < 0;
        
        bool IsBanksSame() => payload.FromBank == payload.ToBank;

        bool IsNotValidDebit() => payload.Type == TransactionType.Debit && payload.FromBank is null;
        
        bool IsNotValidCredit() => payload.Type == TransactionType.Credit && payload.ToBank is null;
        
        if (IsNotValidBanks()) throw new InvalidPayloadException("Invalid payload provided");

        if (IsNotValidAmount()) throw new InvalidPayloadException("Invalid amount provided");
        
        if (IsNotValidDescription()) throw new InvalidPayloadException("Invalid description provided");
        
        if (IsNotValidCategoryId()) throw new InvalidPayloadException("Invalid category Id provided");
        
        if (IsBanksSame()) throw new InvalidPayloadException("Same banks for a transaction is not allowed");
        
        if (IsNotValidDebit()) throw new InvalidPayloadException("Invalid debit for a transaction is not allowed");
        
        if (IsNotValidCredit()) throw new InvalidPayloadException("Invalid credit for a transaction is not allowed");
    }

    private async Task InsertTransactionsMetaAsync(InsertTransactionDto payload, DateOnly currentDate, int transactionId)
    {
        bool ShouldCreateTransactionsMeta() => payload.DueId is not null || payload.EmiId is not null || !string.IsNullOrEmpty(payload.Tags);
        
        if (ShouldCreateTransactionsMeta())
        {
            TransactionsMeta meta = new TransactionsMeta
            {
                TransactionId = transactionId,
                DueId = payload.DueId,
                EmiId = payload.EmiId,
                Tags = payload.Tags,
                CreatedAt = currentDate,
                UpdatedAt = currentDate,
            };

            await _transactionsMetaService.InsertTransactionMetaAsync(meta);
        }
    }

    public async Task InsertTransactionAsync(InsertTransactionDto payload)
    {
        InsertValidations(payload);
        
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
        await InsertTransactionsMetaAsync(payload, currentDate: today, transactionId: transaction.Id);
        await UpdateTransactionsByMonthAsync(payload.Date);
    }

    public async Task<TransactionByDateDto> GetTransactionsByDateAsync(string transactionDate)
    {
        using Activity? activity = _activitySource.StartActivity("GetTransactionsByDate");
        
        if (activity == null)
        {
            _logger.LogWarning("Activity not created!");
        }
        else
        {
            _logger.LogInformation("Activity started!");
        }
        
        return await _repository.GetAllTransactionsByDateAsync(transactionDate);
    }

    public async Task<int> CountOfAllTransactionsAsync(int? month, int? year)
    {
        return await _repository.CountOfAllTransactionsAsync(month, year);
    }

    public async Task UpdateTransactionAsync(UpdateTransactionDto payload, int id)
    {
        InsertValidations(payload);
        
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
            string message = JsonSerializer.Serialize(result);
            
            // TODO: Get Trace ID here
            // Restrict this in "Testing" environment
            await _publisher.PublishMessageAsync(requestMessage: message, eventType: PubSubFinanceEvents.DATEWISE_TRANSACTIONS_LIST, traceId: null);
        }   
    }

    public async Task BigQueryUpdatesAsync()
    {
        List<DateOnly> dates = await _repository.GetGroupOfDatesAsync();

        foreach (DateOnly date in dates)
        {
            await UpdateTransactionsByMonthAsync(date);
        }
    }
}