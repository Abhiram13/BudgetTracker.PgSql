namespace BudgetTracker.Finance.Models;

public record class InsertCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public record class CategoryListDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}