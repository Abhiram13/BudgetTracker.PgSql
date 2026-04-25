using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Finance.Entities;

[Table("transactions_meta")]
public class TransactionsMeta
{
    [Key]
    [Column("transaction_id")]
    [ForeignKey(nameof(Transaction))]
    [JsonPropertyName("transaction_id")]
    public required int TransactionId { get; set; }

    [Column("due_id")]
    [JsonPropertyName("due_id")]
    public int? DueId { get; set; }

    [Column("emi_id")]
    [JsonPropertyName("emi_id")]
    public int? EmiId { get; set; }

    [Column("created_at")]
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [Column("tags")]
    [JsonPropertyName("tags")]
    public string? Tags { get; set; }

    public Transaction TransactionF { get; init; } = default!;
}