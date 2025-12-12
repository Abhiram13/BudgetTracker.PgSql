using BudgetTracker.Dues.Enums;

namespace BudgetTracker.Dues.Models;

public record class InsertDueDto
{
    [JsonPropertyName("debtor")]
    public string Debtor { get; init; } = string.Empty;

    [JsonPropertyName("creditor")]
    public string Creditor { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; init; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; init; } = string.Empty;

    [JsonPropertyName("comments")]
    public string? Comments { get; init; }
}