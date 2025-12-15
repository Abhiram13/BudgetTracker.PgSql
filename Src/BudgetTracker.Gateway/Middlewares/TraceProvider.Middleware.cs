using BudgetTracker.Gateway.Interfaces;

namespace BudgetTracker.Gateway.Middlewares;

public class TraceProviderMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;
    private const string _traceHeader = "x-trace-id";

    public TraceProviderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        string traceId = Guid.NewGuid().ToString();

        httpContext.Request.Headers[_traceHeader] = traceId;
        httpContext.Items[_traceHeader] = traceId;
        httpContext.Response.Headers[_traceHeader] = traceId;

        await _next(httpContext);
    }
}