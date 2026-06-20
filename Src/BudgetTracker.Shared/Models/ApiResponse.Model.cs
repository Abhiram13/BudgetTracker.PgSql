using System.Net;

namespace BudgetTracker.Shared.Models;

/// <summary>
/// Generic API response model with optional message and required result
/// </summary>
public record ApiResponse<T>
{
    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    [JsonPropertyName("status_code")]
    public required HttpStatusCode StatusCode { get; init; } // TODO: Can be changed from System.Net.HttpStatusCode to Microsoft.AspNetCore.Http.StatusCode ?

    /// <summary>
    /// A unique GUID for tracing the request across services.
    /// </summary>
    /// <example>0af7651916cd43dd8448eb211c80319c</example>
    [JsonPropertyName("trace_id")]
    public string TraceId { get; init; } = string.Empty;

    /// <summary>
    /// Generic message of API operations. (Successfully inserted, Insertion failed) or any exception messages
    /// </summary>
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    /// <summary>
    /// The payload of the response.
    /// </summary>
    [JsonPropertyName("result")]
    public required T Result { get; init; }
}

/// <summary>
/// Non-Generic API response that contains no Result with required message
/// </summary>
public record ApiResponse
{
    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    [JsonPropertyName("status_code")]
    public required HttpStatusCode StatusCode { get; init; } // TODO: Can be changed from System.Net.HttpStatusCode to Microsoft.AspNetCore.Http.StatusCode ?

    /// <summary>
    /// A unique GUID for tracing the request across services.
    /// </summary>
    /// <example>0af7651916cd43dd8448eb211c80319c</example>
    [JsonPropertyName("trace_id")]
    public string TraceId { get; init; } = string.Empty;

    /// <summary>
    /// Generic message of API operations. (Successfully inserted, Insertion failed) or any exception messages
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}