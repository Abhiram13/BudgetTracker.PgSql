using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Mvc.Models;

public record TransactionModel
{
    public int Count { get; init; }
    public required IReadOnlyList<TransactionsListByMonthYear> Transactions { get; init; }
}