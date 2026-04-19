using System.Diagnostics;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Gateway.Middlewares;

/// <summary>
/// Passes Trace id in <c>X-Trace-Id</c> request headers to downstream apis
/// </summary>
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

        httpContext.Request.Headers[SharedConstants.Headers.X_TRACE_ID] = traceId;
        httpContext.Items[SharedConstants.Headers.X_TRACE_ID] = traceId;
        httpContext.Response.Headers[SharedConstants.Headers.X_TRACE_ID] = traceId;

        await _next(httpContext);

        _logger.LogInformation("Ending Request = {Path} at Gateway with Trace-Id = {TraceId}", requestPath, traceId);
    }
}