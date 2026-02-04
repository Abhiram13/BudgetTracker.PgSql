namespace BudgetTracker.Finance.Models;

public record InsertBankDto
{
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