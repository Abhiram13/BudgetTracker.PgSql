using System.Net;

namespace BudgetTracker.Shared.Models;

/// <summary>
/// Generic API response model with optional message and optional result
/// </summary>
public record ApiResponse<T>
{
    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    /// <example>201</example>
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
    /// <remarks>
    /// Will be <c>null</c> if the response is successful and there is data in <see cref="Result"/>
    /// </remarks>
    /// <example>Transaction inserted successfully</example>
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    /// <summary>
    /// The payload of the response. This is <c>null</c> if the request fails.
    /// </summary>
    [JsonPropertyName("result")]
    public required T Result { get; init; } // TODO: Make it Required
}

/// <summary>
/// Non-Generic API response that contains no Result
/// </summary>
public record ApiResponse
{
    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    /// <example>201</example>
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
    /// <remarks>
    /// Will be <c>null</c> if the response is successful and there is data in <see cref="Result"/>
    /// </remarks>
    /// <example>Transaction inserted successfully</example>
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
}