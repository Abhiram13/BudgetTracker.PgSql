namespace BudgetTracker.Shared.Models;

public record class DateTransactionsDto
{
    public DateOnly Date { get; init; }
    public double Debit { get; init; }
    public double Credit { get; init; }
}