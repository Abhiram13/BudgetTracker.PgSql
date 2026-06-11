using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Finance.Enums;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Finance.Entities;

[Table("transactions_meta")]
public class TransactionsMeta : BaseTimeStampEntity
{
    [Key]
    [Column("transaction_id")]
    [ForeignKey(nameof(Transaction))]
    public int TransactionId { get; private set; }

    [Column("due_id")]
    public int? DueId { get; private set; }

    [Column("emi_id")]
    public int? EmiId { get; private set; }

    [Column("tags")]
    public string? Tags { get; private set; }

    public Transaction TransactionF { get; private init; } = default!;
    
    private TransactionsMeta() { }
    
    public static TransactionsMeta Create(int transactionId, int? dueId = null, int? emiId = null, string? tags = null)
    {
        TransactionsMeta meta = new TransactionsMeta
        {
            TransactionId = transactionId,
            DueId = dueId,
            EmiId = emiId,
            Tags = tags
        };
        
        meta.SetModifiedAt();
        return meta;
    }

    public void Update(int? transactionId = null, int? dueId = null, int? emiId = null, string? tags = null)
    {
        TransactionId = transactionId ?? TransactionId;
        DueId = dueId;
        EmiId = emiId;
        Tags = tags;
        
        SetUpdatedAt();
    }
}