using System.ComponentModel;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using System.Text.Json;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BudgetTracker.Finance.Services;

/// <summary>
/// Performs business logic for transactions, including validation, transaction metadata, and Outbox event updates.
/// </summary>
/// <remarks>
/// This service ensures atomicity by wrapping multiple operations within a single database transaction. <br /> <br />
/// It is designed to work with the <see cref="ITransactionRepository"/> and uses <see cref="OutboxService"/> to update write events.
/// </remarks>
public class TransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<TransactionService> _logger;
    private readonly TransactionsMetaService _transactionsMetaService;
    private readonly OutboxService _outboxService;
    private readonly WriteDbContext _writeDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionService"/> class.
    /// </summary>
    /// <param name="repository">The <see cref="ITransactionRepository"/> for <see cref="Transaction"/> DB operations.</param>
    /// <param name="logger">The <see cref="ILogger"/> instance for structured logging.</param>
    /// <param name="transactionsMetaService">The <see cref="TransactionsMetaService"/> responsible for managing <see cref="TransactionsMeta"/> DB operations.</param>
    /// <param name="writeDbContext">The primary <see cref="WriteDbContext"/> used for write operations.</param>
    /// <param name="outboxService">The <see cref="OutboxService"/> used to insert events in <see cref="OutboxEvents"/>.</param>
    public TransactionService(
        ITransactionRepository repository, 
        ILogger<TransactionService> logger, 
        TransactionsMetaService transactionsMetaService,
        WriteDbContext writeDbContext,
        OutboxService outboxService
    ) {
        _repository = repository;
        _logger = logger;
        _transactionsMetaService = transactionsMetaService;
        _writeDbContext = writeDbContext;
        _outboxService = outboxService;
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

    /// <summary>
    /// Validates the payload, inserts the <see cref="Transaction"/> and then inserts the <see cref="TransactionsMeta"/>
    /// and then inserts an event in <see cref="OutboxEvents"/> for later processing
    /// </summary>
    /// <remarks>Rollsback all insertions if an exception is thrown</remarks>
    /// <param name="payload"><see cref="InsertTransactionDto"/></param>
    /// <returns><see cref="InsertTransactionResponseDto"/> - An Object containing inserted Transaction ID</returns>
    /// <exception cref="InvalidPayloadException">
    /// Thrown when <paramref name="payload"/> fails validation (e.g., negative amounts, missing required fields, future dates).
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// Thrown if the transaction cannot be inserted in the database, due to any constraint violations
    /// </exception>
    public async Task<InsertTransactionResponseDto> InsertTransactionAsync(InsertTransactionDto payload)
    {
        await using (IDbContextTransaction dbTransaction = await _writeDbContext.Database.BeginTransactionAsync())
        {
            try
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
                _logger.LogInformation("Transaction with Id = {TransactionId} has been inserted successfully", transaction.Id);
                
                await InsertTransactionsMetaAsync(payload, currentDate: today, transactionId: transaction.Id);
                _logger.LogInformation("Transaction meta data with Transaction-Id = {TransactionId} has been inserted successfully", transaction.Id);
                
                await OutboxTransanctionMessageUpdateAsync(payload.Date, transaction.Id);
                await dbTransaction.CommitAsync();

                return new InsertTransactionResponseDto { TransactionId = transaction.Id };
            }
            catch (Exception e)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(e, "Exception at inserting transaction. Rolling back transaction");
                throw;
            }
        }
    }
    
    /// <inheritdoc cref="ITransactionRepository.GetAllTransactionsByDateAsync"/>
    public async Task<TransactionByDateDto> GetTransactionsByDateAsync(string transactionDate)
    {
        return await _repository.GetAllTransactionsByDateAsync(transactionDate);
    }

    /// <inheritdoc cref="ITransactionRepository.CountOfAllTransactionsAsync"/>
    public async Task<int> CountOfAllTransactionsAsync(int? month, int? year)
    {
        return await _repository.CountOfAllTransactionsAsync(month, year);
    }

    /// <inheritdoc cref="ITransactionRepository.UpdateTransactionAsync"/>
    public async Task UpdateTransactionAsync(UpdateTransactionDto payload, int id)
    {
        InsertValidations(payload);
        
        await _repository.UpdateTransactionAsync(payload, id);
        _logger.LogInformation("Transaction with Id = {TransactionId} has been updated successfully", id);

        if (payload.DueId is not null || payload.EmiId is not null || !string.IsNullOrEmpty(payload.Tags))
        {
            UpdateTransactionMetaDto metaDto = new UpdateTransactionMetaDto
            {
                DueId = payload.DueId,
                EmiId = payload.EmiId,
                Tags = payload.Tags,
            };
            await _transactionsMetaService.UpdateTransactionMetaAsync(metaDto, transactionId: id);
            _logger.LogInformation("Transaction meta with Transaction-Id = {TransactionId} has been updated successfully", id);
        }
        
        await OutboxTransanctionMessageUpdateAsync(payload.Date, id);
    }

    private async Task OutboxTransanctionMessageUpdateAsync(DateOnly transactionDate, int transactionId)
    {
        TransactionCreditDebitByDateDto? creditDebitByDate = await _repository.GetDebitCreditByDateAsync(transactionDate);

        if (creditDebitByDate is not null)
        {
            await _outboxService.InsertOneAsync(new OutboxInsertDto
            {
                EntityId = transactionId,
                EventType = OutboxEvents.TRANSACTION_CREATED,
                Status = OutboxStatus.PENDING,
                EntityType = "Transactions",
                Payload = JsonSerializer.SerializeToDocument(creditDebitByDate),
            });
            _logger.LogInformation("Outbox message has been inserted successfully for Event = {EventType} for Transaction-Id = {TransactionId} ", OutboxEvents.TRANSACTION_CREATED, transactionId);
        }
        else
        {
            throw new InvalidPayloadException($"No Debit or Credit is available at Transaction-Id = {transactionId} with Date = {transactionDate}. Credit Debit by date = {creditDebitByDate}");
        }
    }
    
    [Obsolete(message: "Publishing transactions is moved to Outbox pattern. So this method is Obselete", error: true)]
    private async Task UpdateTransactionsByMonthAsync(DateOnly date)
    {
        TransactionCreditDebitByDateDto? result = null;
        
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
            // await _publisher.PublishMessageAsync(requestMessage: message, eventType: PubSubFinanceEvents.DATEWISE_TRANSACTIONS_LIST, traceId: null);
        }   
    }

    /// <summary>
    /// Updates transactions by bulk in big query
    /// </summary>
    /// <remarks><b>OBSOLETE</b> - Should not use unless manually update big query</remarks>
    [Obsolete(message: "Publishing transactions is moved to Outbox pattern. So this method is Obselete", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task BigQueryUpdatesAsync()
    {
        List<DateOnly> dates = await _repository.GetGroupOfDatesAsync();

        foreach (DateOnly date in dates)
        {
            await UpdateTransactionsByMonthAsync(date);
        }
    }
}