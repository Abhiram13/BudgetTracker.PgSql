using System.Text.Json;

namespace BudgetTracker.Finance.Models;

public static class OutboxEvents
{
    public const string TRANSACTION_CREATED = "TransactionCreated";
    public const string TRANSACTION_UPDATED = "TransactionUpdated";
    public const string TRANSACTION_DELETED = "TransactionDeleted";
}

public static class OutboxStatus
{
    public const string PENDING = "Pending";
    public const string SUCCESS = "Success";
    public const string FAILED = "Failed";
    public const string PROCESSING = "Processing";
    public const string PUBLISHED = "Published";
}

public record OutboxInsertDto
{
    public required string EntityType { get; init; }
    public required string EventType { get; init; }
    public required int EntityId { get; init; }
    public required JsonDocument Payload { get; init; }
    public required string Status { get; init; }
    public int? RetryCount { get; init; }
    public string? Error { get; init; }
}

public record OutboxUnProcessedDto
{
    public required JsonDocument Payload { get; init; }
    public Guid Id { get; init; }
}