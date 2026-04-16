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
    [StringLength(maximumLength: LengthConstants.MAX_BANK_LENGTH, MinimumLength = LengthConstants.MIN_BANK_LENGTH, ErrorMessage = "Bank name exceeds or does not reach required length")]
    [RegularExpression(ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces are allowed")]
    public string Name { get; set; } = string.Empty;
}