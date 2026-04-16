using System.ComponentModel.DataAnnotations;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Finance.Models;

public record InsertCategoryDto
{
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(20, ErrorMessage = "Category name exceeds character limit")]
    [MinLength(3, ErrorMessage = "Minimum 3 characters are required")]
    [StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "Category name exceeds or does not reach required length")]
    [RegularExpression(ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
    public string Name { get; init; } = string.Empty;
}

public record CategoryListDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public record CategoryByIdResponseDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}