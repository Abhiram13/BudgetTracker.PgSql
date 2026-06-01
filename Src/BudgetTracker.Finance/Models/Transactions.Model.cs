using System.ComponentModel.DataAnnotations;
using BudgetTracker.Finance.Attributes;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Shared.Constants;
using System.Text.Json.Serialization;

namespace BudgetTracker.Finance.Models;

/// <summary>
/// Payload DTO that will be used by <see cref="InsertTransactionDto"/> and <see cref="UpdateTransactionDto"/> in <see cref="Transaction"/> entity
/// </summary>
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
    [StringLength(maximumLength: SharedConstants.LengthConstants.MAX_TRANSACTION_DESCRIPTION_LENGTH, MinimumLength = SharedConstants.LengthConstants.MIN_TRANSACTION_DESCRIPTION_LENGTH, ErrorMessage = "Description exceeds or does not reach required length")]
    [RegularExpression(SharedConstants.ValidationRegex.DESCRIPTION_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
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

/// <summary>
/// Payload that holds data belongs to Transaction that fetched by Date (<c>yyyy-MM-dd</c>)
/// </summary>
public record TransactionByDateDto
{
    [JsonPropertyName("debit")]
    public decimal Debit { get; init; }

    [JsonPropertyName("credit")]
    public decimal Credit { get; init; }

    [JsonPropertyName("transactions")]
    public IReadOnlyList<Transactions> TransactionsList { get; init; } = new List<Transactions>();

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

public record InsertTransactionResponseDto
{
    [JsonPropertyName("transaction_id")]
    public int TransactionId { get; init; }
}

/// <summary>
/// List of all transactions filtered by given month and year. Each data row contains <c>Debit</c>, <c>Credit</c> and <c>TransactionDate</c>
/// </summary>
public record TransactionsListByMonthYear
{
    /// <summary>
    /// Sum of the debit during a single transaction date
    /// </summary>
    [JsonPropertyName("debit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Debit { get; init; }
    
    /// <summary>
    /// Sum of the credit during a single transaction date
    /// </summary>
    [JsonPropertyName("credit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Credit { get; init; }
    
    /// <summary>
    /// Single transaction date
    /// </summary>
    [JsonIgnore]
    public DateOnly? TransactionDate { get; init; }
    
    [JsonPropertyName("transaction_date")]
    public string FormattedDate
    {
        get
        {
            return TransactionDate.HasValue ? TransactionDate.Value.ToString("ddd, MMM dd yyyy") : string.Empty;
        }
    }
}

public record CategoryBankTransactionsByMonthYear
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }
    
    [JsonPropertyName("amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Amount { get; init; }
}