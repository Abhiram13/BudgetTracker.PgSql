using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface ITransactionsMetaRepository
{
    Task InsertMetaAsync(TransactionsMeta payload);
    Task UpdateMetaAsync(UpdateTransactionMetaDto payload, int transactionId);
}