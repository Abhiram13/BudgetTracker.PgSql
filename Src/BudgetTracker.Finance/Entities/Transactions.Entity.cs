using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Exceptions;

namespace BudgetTracker.Finance.Entities;

[Table("transactions")]
public class Transaction : BaseEntity
{
    [Column("amount")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; private set; }

    [Column("actual_amount")]
    [JsonPropertyName("actual_amount")]
    public decimal? ActualAmount { get; private set; }
    
    [Column("description")]
    [JsonPropertyName("description")]
    public string Description { get; private set; } = string.Empty;

    [Column("from_bank")]    
    [JsonPropertyName("from_bank")]
    public int? FromBank { get; private set; }

    [Column("to_bank")]
    [JsonPropertyName("to_bank")]
    public int? ToBank { get; private set; }
    
    [Column("category_id")]
    [JsonPropertyName("category_id")]
    public int CategoryId { get; private set; }

    [Column(name: "date")]
    [JsonPropertyName("date")]
    public DateOnly Date { get; private set; }
    
    [Column("type")]
    [JsonPropertyName("type")]
    public TransactionType Type { get; private set; }

    // Foreign Key Navigation properties
    [ForeignKey(nameof(CategoryId))]
    public Category CategoryF { get; private set; } = default!;

    [ForeignKey(nameof(FromBank))]
    public Bank FromBankF { get; private set; } = default!;

    [ForeignKey(nameof(ToBank))]
    public Bank ToBankF { get; private set; } = default!;

    public TransactionsMeta Meta { get; private set; } = default!;
    
    private Transaction() { }

    public static Transaction Create(decimal amount, decimal? actualAmount, string description, int categoryId, int? fromBank, int? toBank, TransactionType type, DateOnly date)
    {
        Validate(amount, actualAmount, description, date);
        
        Transaction transaction = new Transaction
        {
            ActualAmount = actualAmount,
            Amount = amount,
            Description = description,
            CategoryId = categoryId,
            Date = date,
            FromBank = fromBank,
            ToBank = toBank,
            Type = type,
        };
        
        transaction.SetModifiedAt();
        
        return transaction;
    }

    public void Update(decimal amount, decimal? actualAmount, string description, int categoryId, int? fromBank, int? toBank, TransactionType type, DateOnly date)
    {
        Validate(amount, actualAmount, description, date);
        
        ActualAmount = actualAmount;
        Amount = amount;
        Description = description;
        CategoryId = categoryId;
        Date = date;
        FromBank = fromBank;
        ToBank = toBank;
        Type = type;
        
        SetUpdatedAt();
    }

    private static void Validate(decimal amount, decimal? actualAmount, string description, DateOnly date)
    {
        if (amount < 0.01m || amount > 1000000)
        {
            throw new InvalidPayloadException("Amount must be between 0.01 and 1000000");
        }
        
        if (actualAmount is not null && (actualAmount < 0.01m || amount > 1000000))
        {
            throw new InvalidPayloadException("Actual Amount must be between 0.01 and 1000000");
        }

        if (string.IsNullOrEmpty(description))
        {
            throw new InvalidPayloadException("Description is required");
        }

        if (!Regex.IsMatch(description, SharedConstants.ValidationRegex.DESCRIPTION_PATTERN))
        {
            throw new InvalidPayloadException("Description contains invalid characters. Only letters, numbers, spaces and # are allowed");
        }

        if (description.Length < SharedConstants.LengthConstants.MIN_TRANSACTION_DESCRIPTION_LENGTH ||
            description.Length > SharedConstants.LengthConstants.MAX_TRANSACTION_DESCRIPTION_LENGTH)
        {
            throw new InvalidPayloadException("Description exceeds or does not reach required length");
        }

        if (date > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new InvalidPayloadException("Date cannot be in the future");
        }
    }
}