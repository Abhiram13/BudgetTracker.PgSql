using System.Diagnostics;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Gateway.Middlewares;

public class ActivityLoggerMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityLoggerMiddleware> _logger;
    private const string _traceHeader = "X-Trace-Id";

    public ActivityLoggerMiddleware(RequestDelegate next, ILogger<ActivityLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        Activity activity = Activity.Current!;
        string traceId = activity.TraceId.ToString();
        string spanId = activity.SpanId.ToString();
        string url = $"{httpContext.Request.Host}{httpContext.Request.Path}";
        
        _logger.LogInformation("Starting Request at Gateway = '{url}', Trace = {TraceId}, Span = {SpanId} ", url, traceId, spanId);
        
        await _next(httpContext);
        
        _logger.LogInformation("Ending Request at Gateway = '{url}', Trace = {TraceId}, Span = {SpanId} ", url, traceId, spanId);
    }
}