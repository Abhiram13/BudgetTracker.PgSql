namespace BudgetTracker.Finance.Models;

public record InsertReceiptDto
{
    public required string FileName { get; init; }
    public required string ObjectKey { get; init; }
    public required string Extension { get; init; }
    public required string MimeType { get; init; }
    public required long FileSize { get; init; }
    public string? MdHash { get; init; }
}