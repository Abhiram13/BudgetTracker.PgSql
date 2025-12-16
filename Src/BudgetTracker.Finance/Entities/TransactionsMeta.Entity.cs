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
    public required int TransactionId { get; init; }

    [Column("due_id")]
    [JsonPropertyName("due_id")]
    public int? DueId { get; init; }

    [Column("emi_id")]
    [JsonPropertyName("emi_id")]
    public int? EmiId { get; init; }

    [Column("created_at")]
    [JsonPropertyName("created_at")]
    public DateOnly CreatedAt { get; init; }

    [Column("updated_at")]
    [JsonPropertyName("updated_at")]
    public DateOnly UpdatedAt { get; init; }

    [Column("tags")]
    [JsonPropertyName("tags")]
    public string? Tags { get; init; }

    public Transaction TransactionF { get; init; } = default!;
}