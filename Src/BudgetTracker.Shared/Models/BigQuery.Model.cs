namespace BudgetTracker.Shared.Models;

public record TransactionCreditDebitByDateDto
{
    [JsonPropertyName("debit")]
    public decimal Debit { get; init; }

    [JsonPropertyName("credit")]
    public decimal Credit { get; init; }

    [JsonPropertyName("date")]
    public DateOnly Date { get; init; } = default!;
    
    [JsonPropertyName("count")]
    public long Count { get; init; }
}