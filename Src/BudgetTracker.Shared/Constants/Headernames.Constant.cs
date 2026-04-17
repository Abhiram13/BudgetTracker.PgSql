namespace BudgetTracker.Shared.Constants;

public static partial class SharedConstants
{
    public static class Headers
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
        [Obsolete("Use JWT")]
        public const string YARP_API_KEY = "YARP_API_KEY";
    
        /// <summary>
        /// Header value used to store trace id
        /// </summary>
        /// <remarks><c>X-Trace-Id</c></remarks>
        public const string X_TRACE_ID = "X-Trace-Id";
    }
}