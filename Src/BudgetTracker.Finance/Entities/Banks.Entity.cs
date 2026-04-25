using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Finance.Entities;

[Table("banks")]
public class Bank : BaseEntity
{
    [Column("name")]
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Bank Name is required")]
    [StringLength(maximumLength: SharedConstants.LengthConstants.MAX_BANK_LENGTH, MinimumLength = SharedConstants.LengthConstants.MIN_BANK_LENGTH, ErrorMessage = "Bank name exceeds or does not reach required length")]
    [RegularExpression(SharedConstants.ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces are allowed")]
    public string Name { get; private set; } = string.Empty;
    
    private Bank() { }

    public static Bank Create(string bankName)
    {
        Bank bank = new Bank { Name = bankName };
        
        bank.SetModifiedAt();
        
        return bank;
    }

    public void Update(string bankName)
    {
        Name = bankName;
        SetUpdatedAt();
    }
}