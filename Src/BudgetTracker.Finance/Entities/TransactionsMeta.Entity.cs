using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;

namespace BudgetTracker.Finance.Entities;

[Table("transactions_meta")]
public class TransactionsMeta
{
    [Column("transaction_id")]
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
    public DateTime CreatedAt { get; init; }

    [Column("updated_at")]
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; init; }

    [Column("tags")]
    [JsonPropertyName("tags")]
    public string? Tags { get; init; }

    [ForeignKey(nameof(TransactionId))]
    public Transaction TransactionF { get; init; } = default!;
}