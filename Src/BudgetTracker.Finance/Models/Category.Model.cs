using System.ComponentModel.DataAnnotations;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Finance.Models;

/// <summary>
/// DTO that holds category name recieved from HTTP request payload
/// </summary>
public record InsertCategoryDto
{
    /// <summary>
    /// Category Name
    /// </summary>
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(maximumLength: SharedConstants.LengthConstants.MAX_CATEGORY_LENGTH, MinimumLength = SharedConstants.LengthConstants.MIN_CATEGORY_LENGTH, ErrorMessage = "Category name exceeds or does not reach required length")]
    [RegularExpression(SharedConstants.ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
    public string Name { get; init; } = string.Empty;
}

/// <summary>
/// DTO defines category details
/// </summary>
public record CategoryDto
{
    /// <summary>
    /// Category ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    /// <summary>
    /// Category Name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}