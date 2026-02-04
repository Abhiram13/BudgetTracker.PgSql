using System.Net;

namespace BudgetTracker.Shared.Models;

public record class ApiResponse<T>
{
    [JsonPropertyName("status_code")]
    public required HttpStatusCode StatusCode { get; init; }

    [JsonPropertyName("trace_id")]
    public string TraceId { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    [JsonPropertyName("result")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Result { get; init; }
}