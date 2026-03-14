using System.ComponentModel.DataAnnotations;
using BudgetTracker.Finance.Attributes;
using BudgetTracker.Finance.Enums;

namespace BudgetTracker.Finance.Models;

public record InsertTransactionDto
{
    [Range(type: typeof(decimal), minimum: "0.01",  maximum: "1000000")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    
    [Range(type: typeof(decimal), minimum: "0",  maximum: "1000000")]
    [JsonPropertyName("actual_amount")]
    public decimal? ActualAmount { get; init; }
    
    [StringLength(maximumLength: 50, MinimumLength = 1)]
    [JsonPropertyName("description")]
    [RegularExpression(@"^[a-zA-Z0-9#,\s]*$", ErrorMessage = "Only letters, numbers and spaces allowed")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("from_bank")]
    public int? FromBank { get; init; }

    [JsonPropertyName("to_bank")]
    public int? ToBank { get; init; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; init; }

    [JsonPropertyName("date")]
    [MaxDate(ErrorMessage = "Provided date is out of range or invalid.")]
    public DateOnly Date { get; init; }

    [JsonPropertyName("type")]
    public TransactionType Type { get; init; }
    
    [JsonPropertyName("due_id")]
    public int? DueId { get; init; }
    
    [JsonPropertyName("emi_id")]
    public int? EmiId { get; init; }
    
    [JsonPropertyName("tags")]
    public string? Tags { get; init; }
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

public record UpdateTransactionDto
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("actual_amount")]
    public decimal? ActualAmount { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("from_bank")]
    public int? FromBank { get; init; }

    [JsonPropertyName("to_bank")]
    public int? ToBank { get; init; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; init; }

    [JsonPropertyName("date")]
    [MaxDate(ErrorMessage = "Provided date is out of range or invalid.")]  
    public DateOnly Date { get; init; }

    [JsonPropertyName("type")]
    public TransactionType Type { get; init; }
    
    [JsonPropertyName("due_id")]
    public int? DueId { get; init; }
    
    [JsonPropertyName("emi_id")]
    public int? EmiId { get; init; }
    
    [JsonPropertyName("tags")]
    public string? Tags { get; init; }
}