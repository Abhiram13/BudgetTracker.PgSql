namespace BudgetTracker.Shared.Constants;

/// <summary>
/// Provides a centralized collection of standard HTTP header names used across the application.
/// </summary>
public static class HeaderNames
{
    /// <summary>
    /// Header value used to authenticate to Gateway apis
    /// </summary>
    /// <remarks><c>API_KEY</c></remarks>
    public const string API_KEY = "API_KEY";
    
    /// <summary>
    /// Header value used to authenticate to Downstream apis
    /// </summary>
    /// <remarks><c>YARP_API_KEY</c></remarks>
    public const string YARP_API_KEY = "YARP_API_KEY";
    
    /// <summary>
    /// Header value used to store trace id
    /// </summary>
    /// <remarks><c>X-Trace-Id</c></remarks>
    public const string X_TRACE_ID = "X-Trace-Id";
}