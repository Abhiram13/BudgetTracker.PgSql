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
        TransactionListDto? result = await _repository.GetDebitCreditByDateAsync(payload.Date.ToString("yyyy-MM-dd"));
        
        if (result is not null)
        {
            await _bigQueryService.InsertTransactionsByDateAsync(new DateTransactionsDto { 
                Credit = (double) result.Credit, 
                Debit = (double) result.Debit, 
                Date = DateTime.Parse(result.Date) 
            });
        }

        return transaction;
    }

    public async Task<List<TransactionListDto>> GetAllTransactionsAsync(int? month, int? year)
    {
        void CheckMonthAndDate()
        {
            try
            {
                if (year is not null && month is not null)
                {
                    _ = new DateOnly((int) year, (int) month, 1);
                    return;
                }

                if (month is not null)
                {
                    _ = new DateOnly(2000, (int) month, 1);
                    return;             
                }

                if (year is not null)
                {
                    _ = new DateOnly((int) year, 01, 1);
                    return;
                }                
            }
            catch (Exception e)
            {
                throw new InvalidPayloadException(e.Message);
            }
        }

        CheckMonthAndDate();
        return await _repository.GetTransactionsAsync();
    }

    public async Task<TransactionByDateDto> GetTransactionsByDateAsync(string transactionDate)
    {
        return await _repository.GetAllTransactionsByDateAsync(transactionDate);
    }
}