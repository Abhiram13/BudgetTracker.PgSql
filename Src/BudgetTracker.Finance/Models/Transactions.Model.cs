namespace BudgetTracker.Finance.Models;

public record class InsertTransactionDto
{
    public decimal Amount { get; set; }
    public decimal ActualAmount { get; set; }
    public string Description { get; set; } = string.Empty;
}