namespace BudgetTracker.Shared.Models;

public record class DateTransactionsDto
{
    public DateTime Date { get; init; }
    public double Debit { get; init; }
    public double Credit { get; init; }
}