namespace BudgetTracker.Finance.Models;

public record class InsertBankDto
{
    public string Name { get; set; } = string.Empty;
}

public record class BankListDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}