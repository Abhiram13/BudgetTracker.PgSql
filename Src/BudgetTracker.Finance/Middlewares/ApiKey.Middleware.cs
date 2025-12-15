using BudgetTracker.Shared.Interfaces;

namespace BudgetTracker.Gateway.Middlewares;

public class ApiKeyMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        string? apiKey = Environment.GetEnvironmentVariable("YARP_API_KEY");
        string? xTraceId = httpContext.Request.Headers["X-Trace-Id"];

        Console.WriteLine(apiKey);
        Console.WriteLine(xTraceId);

        if (string.IsNullOrEmpty(xTraceId) || xTraceId != apiKey)
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new
            {
                Error = "Unauthorized",
                Message = "Invalid auth key provided"
            });
            return;
        }

        await _next(httpContext);
    }
}