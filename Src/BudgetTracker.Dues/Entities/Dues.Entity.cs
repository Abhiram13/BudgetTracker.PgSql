using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Dues.Enums;

namespace BudgetTracker.Dues.Entities;

[Table("dues")]
public class Due : BaseEntity
{
    [Column("debtor")]
    [JsonPropertyName("debtor")]
    [Comment(comment: "Person who owes money.")]
    public string Debtor { get; set; } = string.Empty;

    [Column("creditor")]
    [JsonPropertyName("creditor")]
    [Comment(comment: "Money is owed to this person")]
    public string Creditor { get; set; } = string.Empty;

    [Column("description")]
    [JsonPropertyName("description")]
    [Comment(comment: "Description of what the Due is")]    
    public string Description { get; set; } = string.Empty;

    [Column("title")]
    [JsonPropertyName("title")]
    [Comment(comment: "Due short title")]
    public string Title { get; set; } = string.Empty;

    [Column("total_amount")]
    [JsonPropertyName("total_amount")]
    [Comment(comment: "Total amount due to Creditor")]
    public decimal TotalAmount { get; set; }

    [Column("due_amount")]
    [JsonPropertyName("due_amount")]
    [Comment(comment: "Leftover due amount")]
    public decimal DueAmount { get; set; }

    [Column("status")]
    [JsonPropertyName("status")]
    public DueType Status { get; set; } = DueType.Active;

    [Column("start_date")]
    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [Column("completed_date")]
    [JsonPropertyName("completed_date")]
    public DateTime CompletedDate { get; set; }

    [Column("remarks")]
    [JsonPropertyName("remarks")]
    [Comment(comment: "Additional info apert from description to describe the payment of the dues")]
    public string Remarks { get; set; } = string.Empty;

    [Column("comments")]
    [JsonPropertyName("comments")]
    [Comment(comment: "Comments to describe each update action on due")]
    public string Comments { get; set; } = string.Empty;
}