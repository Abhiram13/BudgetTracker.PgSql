using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Gateway.Middlewares;

public class TraceProviderMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TraceProviderMiddleware> _logger;
    private const string _traceHeader = "X-Trace-Id";

    public TraceProviderMiddleware(RequestDelegate next, ILogger<TraceProviderMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        TraceIdProvider traceIdProvider = httpContext.RequestServices.GetRequiredService<TraceIdProvider>();
        string traceId = traceIdProvider.TraceId;

        _logger.LogInformation("Starting Request at Gateway = '{0}' with Trace-Id = '{1}'", $"{httpContext.Request.Host}{httpContext.Request.Path}", traceId);

        httpContext.Request.Headers[_traceHeader] = traceId;
        httpContext.Items[_traceHeader] = traceId;
        httpContext.Response.Headers[_traceHeader] = traceId;

        await _next(httpContext);

        _logger.LogInformation("Ending Request at Gateway = '{0}' with Trace-Id = '{1}'", $"{httpContext.Request.Host}{httpContext.Request.Path}", traceId);
    }
}