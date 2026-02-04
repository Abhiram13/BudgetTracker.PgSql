using BudgetTracker.Finance.Attributes;
using BudgetTracker.Finance.Enums;

namespace BudgetTracker.Finance.Models;

public record InsertTransactionDto
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("actual_amount")]
    public decimal? ActualAmount { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("from_bank")]
    public int? FromBank { get; init; }

    [JsonPropertyName("to_bank")]
    public int? ToBank { get; init; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; init; }

    [JsonPropertyName("date")]
    [MaxDate(ErrorMessage = "Provided date is out of range or invalid.")] // BUG: Getting error for today's date 
    public DateOnly Date { get; init; }

    [JsonPropertyName("type")]
    public TransactionType Type { get; init; }
    
    [JsonPropertyName("due_id")]
    public int? DueId { get; init; }
    
    [JsonPropertyName("emi_id")]
    public int? EmiId { get; init; }
}

public record TransactionByDateDto
{
    [JsonPropertyName("debit")]
    public decimal Debit { get; init; }

    [JsonPropertyName("credit")]
    public decimal Credit { get; init; }

    [JsonPropertyName("transactions")]
    public List<Transactions> TransactionsList { get; init; } = new List<Transactions>();

    public record Transactions
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("type")]
        public TransactionType Type { get; init; }
        
        [JsonPropertyName("id")]
        public int TransactionId { get; init; }
        
        [JsonPropertyName("description")]
        public string Description { get; init; } = string.Empty;
    }
}

public record class CategoryTransactionsSumDto
{
    public string CategoryName { get; init; } = string.Empty;
    public decimal CurrentMonth { get; init; }
    public decimal PreviousMonth { get; init; }
    public decimal Difference { get; init; }
}