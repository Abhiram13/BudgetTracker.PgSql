using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;

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
}