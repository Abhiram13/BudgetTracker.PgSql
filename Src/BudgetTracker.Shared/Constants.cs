namespace BudgetTracker.Shared.Constants;

public static class HeaderNames
{
    /// <summary>
    /// Header value used to authenticate to Gateway apis
    /// </summary>
    public const string API_KEY = "API_KEY";
    
    /// <summary>
    /// Header value used to authenticate to Downstream apis
    /// </summary>
    public const string YARP_API_KEY = "YARP_API_KEY";
    
    /// <summary>
    /// Header value used to store trace id
    /// </summary>
    public const string X_TRACE_ID = "X-Trace-Id";
}