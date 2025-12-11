using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Exceptions;

namespace BudgetTracker.Finance.Services;

public class TransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Transaction> InsertTransactionAsync(Transaction payload)
    {
        Transaction transaction = await _repository.InsertOneTransactionAsync(payload);
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
}