using System.ComponentModel.DataAnnotations;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Finance.Models;

public record InsertBankDto
{
    [Required(ErrorMessage = "Bank name is required")]
    [MaxLength(20, ErrorMessage = "Bank name exceeds character limit")]
    [MinLength(3, ErrorMessage = "Minimum 3 characters are required")]
    [StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "Bank name exceeds or does not reach required length")]
    [RegularExpression(ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
    public string Name { get; set; } = string.Empty;
}

public record BankListDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public record BankByIdResponseDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}