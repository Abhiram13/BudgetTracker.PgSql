using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Shared.Entities;
using Google.Type;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Entities;

[Keyless]
[Table("balances")]
public sealed class Balances : BaseEntity
{
    [Column("date")]
    public DateOnly Date { get; private set; }
    
    [Column("bank")]
    public string Bank { get; private set; } = string.Empty;
    
    [Column("opening_balance")]
    public decimal OpeningBalance { get; private set; }
    
    [Column("closing_balance")]
    public decimal ClosingBalance { get; private set; }
    
    // Balance name
    // Balance description
    // Banks to be included
    
    private Balances() { }

    public static Balances Create(DateOnly date, string bank, decimal openingBalance, decimal closingBalance = 0)
    {
        Balances balance = new Balances
        {
            Bank = bank,
            Date = date,
            OpeningBalance = openingBalance,
            ClosingBalance = closingBalance
        };
        
        balance.SetModifiedAt();
        
        return balance;
    }
}
