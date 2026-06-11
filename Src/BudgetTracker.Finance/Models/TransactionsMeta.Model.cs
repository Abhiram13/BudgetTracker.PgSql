namespace BudgetTracker.Finance.Models;

public record InsertTransactionMetaDto(int? EmiId, int? DueId, int TransactionId, string? Tags);

public record UpdateTransactionMetaDto
{
    public int? EmiId { get; init; }
    public int? DueId { get; init; }
    public string? Tags { get; init; }
}