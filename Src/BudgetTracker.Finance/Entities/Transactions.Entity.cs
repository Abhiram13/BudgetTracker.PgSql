using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;

namespace BudgetTracker.Finance.Entities;

[Table("transactions")]
// TODO: Change Set to Init
public class Transaction : BaseEntity
{
    [Column("amount")]
    [JsonPropertyName("amount")]
    [Comment(comment: "Amount that was used in a transaction")]
    public decimal Amount { get; set; }

    [Column("actual_amount")]
    [JsonPropertyName("actual_amount")]
    [Comment(comment: "Amount that was used in a transaction and left the bank account. Credit card transaction amounts won't be added in actual amount")]
    public decimal? ActualAmount { get; set; }

    [Column("description")]
    [JsonPropertyName("description")]
    // TODO: Setup character limit
    public string Description { get; set; } = string.Empty;

    [Column("from_bank")]    
    [JsonPropertyName("from_bank")]
    public int? FromBank { get; set; }

    [Column("to_bank")]
    [JsonPropertyName("to_bank")]
    public int? ToBank { get; set; }

    [Column("category_id")]
    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [Column(name: "date")]
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

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