using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Finance.Entities;

[Table("categories")]
public class Category : BaseEntity
{
    [Column("name")]
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Category Name is required")]
    [MaxLength(LengthConstants.MAX_CATEGORY_LENGTH, ErrorMessage = "Category name exceeds character limit")]
    [MinLength(LengthConstants.MIN_CATEGORY_LENGTH, ErrorMessage = $"Minimum characters are required")]
    [StringLength(maximumLength: LengthConstants.MAX_CATEGORY_LENGTH, MinimumLength = LengthConstants.MIN_CATEGORY_LENGTH, ErrorMessage = "Category name exceeds or does not reach required length")]
    [RegularExpression(ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces are allowed")]
    public required string Name { get; set; } = string.Empty;
}