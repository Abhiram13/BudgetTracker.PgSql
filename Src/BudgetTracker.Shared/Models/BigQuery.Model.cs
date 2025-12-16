namespace BudgetTracker.Shared.Models;

public record class TransactionsListByMonthDto
{
    [JsonPropertyName("debit")]
    public decimal Debit { get; init; }

    [JsonPropertyName("credit")]
    public decimal Credit { get; init; }

    [JsonPropertyName("date")]
    public DateOnly Date { get; init; } = default!;
}