using BudgetTracker.Finance.Enums;

namespace BudgetTracker.Finance.Models;

public record class InsertTransactionDto
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("actual_amount")]
    public decimal ActualAmount { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("from_bank")]
    public int FromBank { get; init; }

    [JsonPropertyName("to_bank")]
    public int ToBank { get; init; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; init; }

    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public TransactionType Type { get; init; }
}

public record class TransactionListDto
{
    [JsonPropertyName("debit")]
    public decimal Debit { get; init; }

    [JsonPropertyName("credit")]
    public decimal Credit { get; init; }

    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;
}

public record class TransactionByDateDto
{
    [JsonPropertyName("debit")]
    public decimal Debit { get; init; }

    [JsonPropertyName("credit")]
    public decimal Credit { get; init; }

    [JsonPropertyName("transactions")]
    public List<Transactions> TransactionsList { get; init; } = new List<Transactions>();

    public record class Transactions
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("type")]
        public TransactionType Type { get; init; }
    }
}