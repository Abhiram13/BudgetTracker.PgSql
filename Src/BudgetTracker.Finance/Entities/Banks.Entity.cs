using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Exceptions;

namespace BudgetTracker.Finance.Entities;

[Table("banks")]
public class Bank : BaseEntity
{
    [Column("name")]
    public string Name { get; private set; } = string.Empty;
    
    private Bank() { }

    public static Bank Create(string bankName)
    {
        Validate(bankName);
        
        Bank bank = new Bank { Name = bankName };
        bank.SetModifiedAt();
        
        return bank;
    }

    public void Update(string bankName)
    {
        Validate(bankName);
        
        Name = bankName;
        SetUpdatedAt();
    }

    private static void Validate(string bankName)
    {
        if (string.IsNullOrEmpty(bankName))
        {
            throw new InvalidPayloadException("Bank name is required");
        }
        
        if (bankName.Length < SharedConstants.LengthConstants.MIN_BANK_LENGTH || bankName.Length > SharedConstants.LengthConstants.MAX_BANK_LENGTH)
        {
            throw new InvalidPayloadException("Bank name exceeds or does not reach required length");
        }

        if (!Regex.IsMatch(bankName, SharedConstants.ValidationRegex.NAME_PATTERN))
        {
            throw new InvalidPayloadException("Bank name contains invalid characters. Only letters, numbers, spaces are allowed");
        }
    }
}