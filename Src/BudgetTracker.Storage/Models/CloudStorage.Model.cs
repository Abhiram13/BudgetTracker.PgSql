namespace BudgetTracker.Storage.Models;

public record UploadFileDto
{
    [JsonPropertyName("transaction_id")]
    public required int TransactionId { get; init; }
    
    [JsonPropertyName("file_name")]
    public required string FileName { get; init; }
    
    [JsonPropertyName("content_type")]
    public required string ContentType { get; init; }
}