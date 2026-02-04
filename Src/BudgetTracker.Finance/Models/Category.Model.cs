namespace BudgetTracker.Finance.Models;

public record InsertCategoryDto
{
    public string Name { get; set; } = string.Empty;
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