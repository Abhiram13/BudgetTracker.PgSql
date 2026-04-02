using System.Net;

namespace BudgetTracker.Shared.Models;

public record ApiResponse<T>
{
    [JsonPropertyName("status_code")]
    public required HttpStatusCode StatusCode { get; init; } // TODO: Can be changed from System.Net.HttpStatusCode to Microsoft.AspNetCore.Http.StatusCode ?

    [JsonPropertyName("trace_id")]
    public string TraceId { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    [JsonPropertyName("result")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Result { get; init; }
}