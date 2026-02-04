namespace BudgetTracker.Finance.Models;

public record InsertTransactionMetaDto
{
    public int? EmiId { get; init; }
    public int? DueId { get; init; }
    public required int TransactionId { get; init; }
    public string? Tags { get; init; }
}

public record UpdateTransactionMetaDto
{
    public int? EmiId { get; init; }
    public int? DueId { get; init; }
    public string? Tags { get; init; }
}