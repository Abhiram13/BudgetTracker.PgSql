using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Entities;

[Table("dues")]
public class Due : BaseEntity
{
    [Column("debtor")]
    [Comment(comment: "Person who owes money.")]
    public string Debtor { get; private set; }

    [Column("creditor")]
    [Comment(comment: "Money is owed to this person")]
    public string Creditor { get; private set; }

    [Column("description")]
    [Comment(comment: "Description of what the Due is")]    
    public string Description { get; private set; }

    [Column("title")]
    [Comment(comment: "Due short title")]
    public string Title { get; private set; }

    [Column("total_amount")]
    [Comment(comment: "Total amount due to Creditor")]
    public decimal TotalAmount { get; private set; }

    [Column("due_amount")]
    [Comment(comment: "Leftover due amount")]
    public decimal DueAmount { get; private set; }

    [Column("status")]
    public DueType Status { get; private set; } = DueType.Active;

    [Column("start_date")]
    public DateOnly StartDate { get; private set; }

    [Column("completed_date")]
    public DateOnly? CompletedDate { get; private set; }

    [Column("remarks")]
    [Comment(comment: "Additional info apert from description to describe the payment of the dues")]
    public string? Remarks { get; private set; }

    [Column("comments")]
    [Comment(comment: "Comments to describe each update action on due")]
    public string? Comments { get; private set; }
    
    private Due() { }

    public static Due Create(string? debtor, string? creditor, string description, string title, decimal totalAmount, DateOnly startDate, string? remarks = null, string? comments = null)
    {
        // TODO: Validations
        Due due = new Due
        {
            Debtor = debtor ?? "",
            Creditor = creditor ?? "",
            Description = description,
            Title = title,
            TotalAmount = totalAmount,
            DueAmount = totalAmount,
            CompletedDate = null,
            StartDate = startDate,
            Status = DueType.Active,
            Comments = comments,
            Remarks = remarks
        };

        return due;
    }
}