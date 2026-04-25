using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Exceptions;

namespace BudgetTracker.Finance.Entities;

[Table("transactions")]
public class Transaction : BaseEntity
{
    [Required(ErrorMessage = "Transaction Amount is required")]
    [Column("amount")]
    [JsonPropertyName("amount")]
    [Comment(comment: "Amount that will be used in a transaction")]
    [Range(type: typeof(decimal), minimum: "0.01",  maximum: "1000000", ErrorMessage = "Given amount is greater than limit")]
    public decimal Amount { get; private set; }

    [Column("actual_amount")]
    [JsonPropertyName("actual_amount")]
    [Range(type: typeof(decimal), minimum: "0.01",  maximum: "1000000", ErrorMessage = "Given actual amount is greater than limit")]
    [Comment(comment: "Amount that will be used in a transaction and left the bank account. Credit card transaction amounts won't be added in actual amount")]
    public decimal? ActualAmount { get; private set; }
    
    [Column("description")]
    [JsonPropertyName("description")]
    [Required(ErrorMessage = "Description is required")]
    [StringLength(maximumLength: SharedConstants.LengthConstants.MAX_TRANSACTION_DESCRIPTION_LENGTH, MinimumLength = SharedConstants.LengthConstants.MIN_TRANSACTION_DESCRIPTION_LENGTH, ErrorMessage = "Description exceeds or does not reach required length")]
    [RegularExpression(SharedConstants.ValidationRegex.DESCRIPTION_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
    public string Description { get; private set; } = string.Empty;

    [Column("from_bank")]    
    [JsonPropertyName("from_bank")]
    public int? FromBank { get; private set; }

    [Column("to_bank")]
    [JsonPropertyName("to_bank")]
    public int? ToBank { get; private set; }

    [Required(ErrorMessage = "Category Id is required")]
    [Column("category_id")]
    [JsonPropertyName("category_id")]
    public int CategoryId { get; private set; }

    [Column(name: "date")]
    [JsonPropertyName("date")]
    public DateOnly Date { get; private set; }

    [Required(ErrorMessage = "Transaction type is required")]
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
        if (amount < 0.01m || amount > 1000000)
        {
            throw new InvalidPayloadException("Amount must be between 0.01 and 1000000");
        }
        
        if (actualAmount is not null && (actualAmount < 0.01m || amount > 1000000))
        {
            throw new InvalidPayloadException("Actual Amount must be between 0.01 and 1000000");
        }
        
        //TODO: more validations here
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
}