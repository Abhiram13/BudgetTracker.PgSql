using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Utilities;

namespace BudgetTracker.Gateway.Middlewares;

public class TraceProviderMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;
    private const string _traceHeader = "X-Trace-Id";

    public TraceProviderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        TraceIdProvider traceIdProvider = httpContext.RequestServices.GetRequiredService<TraceIdProvider>();
        string traceId = traceIdProvider.TraceId;

        httpContext.Request.Headers[_traceHeader] = traceId;
        httpContext.Items[_traceHeader] = traceId;
        httpContext.Response.Headers[_traceHeader] = traceId;

        await _next(httpContext);
    }
}