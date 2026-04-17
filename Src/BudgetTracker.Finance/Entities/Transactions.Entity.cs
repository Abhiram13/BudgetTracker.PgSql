using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Finance.Entities;

[Table("transactions")]
public class Transaction : BaseEntity
{
    [Required(ErrorMessage = "Transaction Amount is required")]
    [Column("amount")]
    [JsonPropertyName("amount")]
    [Comment(comment: "Amount that will be used in a transaction")]
    [Range(type: typeof(decimal), minimum: "0.01",  maximum: "1000000", ErrorMessage = "Given amount is greater than limit")]
    public decimal Amount { get; set; }

    [Column("actual_amount")]
    [JsonPropertyName("actual_amount")]
    [Range(type: typeof(decimal), minimum: "0.01",  maximum: "1000000", ErrorMessage = "Given actual amount is greater than limit")]
    [Comment(comment: "Amount that will be used in a transaction and left the bank account. Credit card transaction amounts won't be added in actual amount")]
    public decimal? ActualAmount { get; set; }
    
    [Column("description")]
    [JsonPropertyName("description")]
    [Required(ErrorMessage = "Description is required")]
    [StringLength(maximumLength: SharedConstants.LengthConstants.MAX_TRANSACTION_DESCRIPTION_LENGTH, MinimumLength = SharedConstants.LengthConstants.MIN_TRANSACTION_DESCRIPTION_LENGTH, ErrorMessage = "Description exceeds or does not reach required length")]
    [RegularExpression(SharedConstants.ValidationRegex.DESCRIPTION_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
    public string Description { get; set; } = string.Empty;

    [Column("from_bank")]    
    [JsonPropertyName("from_bank")]
    public int? FromBank { get; set; }

    [Column("to_bank")]
    [JsonPropertyName("to_bank")]
    public int? ToBank { get; set; }

    [Required(ErrorMessage = "Category Id is required")]
    [Column("category_id")]
    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [Column(name: "date")]
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Transaction type is required")]
    [Column("type")]
    [JsonPropertyName("type")]
    public TransactionType Type { get; set; }

    // Foreign Key Navigation properties
    [ForeignKey(nameof(CategoryId))]
    public Category CategoryF { get; set; } = default!;

    [ForeignKey(nameof(FromBank))]
    public Bank FromBankF { get; set; } = default!;

    [ForeignKey(nameof(ToBank))]
    public Bank ToBankF { get; set; } = default!;

    public TransactionsMeta Meta { get; set; } = default!;
}