namespace BudgetTracker.Finance.Models;

public sealed record InsertDueDto
{
    [JsonPropertyName("debtor")]
    public string? Debtor { get; init; }
    
    [JsonPropertyName("creditor")]
    public string? Creditor { get; init; }
    
    [JsonPropertyName("description")]
    public required string Description { get; init; }
    
    [JsonPropertyName("title")]
    public required string Title { get; init; }
    
    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; init; }
    
    [JsonPropertyName("start_date")]
    public DateOnly StartDate { get; init; }
    
    [JsonPropertyName("remarks")]
    public string? Remarks { get; init; } = null;

    [JsonPropertyName("comments")] 
    public string? Comments { get; init; } = null;
}