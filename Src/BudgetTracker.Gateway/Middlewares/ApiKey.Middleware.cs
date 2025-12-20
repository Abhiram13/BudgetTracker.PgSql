using BudgetTracker.Shared.Interfaces;

namespace BudgetTracker.Gateway.Middlewares;

public class ApiKeyMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // TODO: Add loggers
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // TODO: Change to GCP Secrets
        string? apiKey = Environment.GetEnvironmentVariable("YARP_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Something went wrong",
            });
            return;
        }

        httpContext.Request.Headers["YARP_API_KEY"] = apiKey;

        await _next(httpContext);
    }
}