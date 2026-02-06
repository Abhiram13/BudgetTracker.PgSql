using BudgetTracker.Shared.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BudgetTracker.Shared.Middlwares;

public class ValidateTraceIdMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;

    public ValidateTraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        string? traceId = httpContext.Request.Headers["X-Trace-Id"]; // TODO: How to validate Trace ID?

        if (string.IsNullOrEmpty(traceId))
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid request provided",
            });
            return;
        }

        await _next(httpContext);
    }
}