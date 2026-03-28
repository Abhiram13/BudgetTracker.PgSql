using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Finance.Models;

public record InsertCategoryDto
{
    [JsonPropertyName("name")]
    [StringLength(maximumLength: 50, MinimumLength = 1)]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z0-9#,\s]*$", ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
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