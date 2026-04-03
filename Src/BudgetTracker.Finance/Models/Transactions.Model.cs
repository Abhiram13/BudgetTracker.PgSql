using System.ComponentModel.DataAnnotations;
using BudgetTracker.Finance.Attributes;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Finance.Models;

public abstract record TransactionDto
{
    [Required(ErrorMessage = "Amount is required")]
    [Range(type: typeof(decimal), minimum: "0.01",  maximum: "1000000", ErrorMessage = "Given amount is greater than limit")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    
    [Range(type: typeof(decimal), minimum: "0",  maximum: "1000000", ErrorMessage = "Given amount is greater than limit")]
    [JsonPropertyName("actual_amount")]
    public decimal? ActualAmount { get; init; }
    
    [JsonPropertyName("description")]
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(50, ErrorMessage = "Description exceeds character limit")]
    [MinLength(3, ErrorMessage = "Minimum 3 characters are required")]
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Description exceeds or does not reach required length")]
    [RegularExpression(ValidationRegex.DESCRIPTION_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("from_bank")]
    public int? FromBank { get; init; }

    [JsonPropertyName("to_bank")]
    public int? ToBank { get; init; }

    [Required(ErrorMessage = "Category Id is required")]
    [JsonPropertyName("category_id")]
    public int CategoryId { get; init; }

    [JsonPropertyName("date")]
    [MaxDate(ErrorMessage = "Provided date is out of range or invalid.")]
    public DateOnly Date { get; init; }

    [Required(ErrorMessage = "Transaction type is required")]
    [JsonPropertyName("type")]
    public TransactionType Type { get; init; }
    
    [JsonPropertyName("due_id")]
    public int? DueId { get; init; }
    
    [JsonPropertyName("emi_id")]
    public int? EmiId { get; init; }
    
    [JsonPropertyName("tags")]
    public string? Tags { get; init; }
}

public record InsertTransactionDto : TransactionDto { }

public record UpdateTransactionDto : TransactionDto { }

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