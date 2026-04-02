using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Gateway.Middlewares;

public class TraceProviderMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TraceProviderMiddleware> _logger;

    public TraceProviderMiddleware(RequestDelegate next, ILogger<TraceProviderMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        TraceIdProvider traceIdProvider = httpContext.RequestServices.GetRequiredService<TraceIdProvider>();
        string traceId = traceIdProvider.TraceId; // FIXME: TraceIds here and at downstream apis are not matching.
        string requestPath = $"{httpContext.Request.Host}{httpContext.Request.Path}";

        _logger.LogInformation("Starting Request = {Path} at Gateway with Trace-Id = {TraceId}", requestPath, traceId);

        httpContext.Request.Headers[HeaderNames.X_TRACE_ID] = traceId;
        httpContext.Items[HeaderNames.X_TRACE_ID] = traceId;
        httpContext.Response.Headers[HeaderNames.X_TRACE_ID] = traceId;

        await _next(httpContext);

        _logger.LogInformation("Ending Request = {Path} at Gateway with Trace-Id = {TraceId}", requestPath, traceId);
    }
}