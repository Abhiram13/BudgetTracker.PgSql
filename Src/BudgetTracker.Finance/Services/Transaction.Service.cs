using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Exceptions;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Services;

public class TransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly BigQueryService _bigQueryService;

    public TransactionService(ITransactionRepository repository, BigQueryService bigQueryService)
    {
        _repository = repository;
        _bigQueryService = bigQueryService;
    }

    public async Task<Transaction> InsertTransactionAsync(Transaction payload)
    {
        Transaction transaction = await _repository.InsertOneTransactionAsync(payload);
        TransactionListDto<DateTime>? result = await _repository.GetDebitCreditByDateAsync(payload.Date);
        
        if (result is not null)
        {
            await _bigQueryService.InsertTransactionsByDateAsync(new DateTransactionsDto { 
                Credit = (double) result.Credit, 
                Debit = (double) result.Debit, 
                Date = result.Date
            });
        }

        return transaction;
    }

    public async Task<TransactionByDateDto> GetTransactionsByDateAsync(string transactionDate)
    {
        return await _repository.GetAllTransactionsByDateAsync(transactionDate);
    }
}