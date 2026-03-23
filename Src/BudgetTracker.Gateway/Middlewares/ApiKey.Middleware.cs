using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Gateway.Middlewares;

public class ApiKeyMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly YarpApiKeySecret _yarpAppSecret;

    public ApiKeyMiddleware(RequestDelegate next, YarpApiKeySecret yarpAppSecret)
    {
        _next = next;
        _yarpAppSecret = yarpAppSecret;
    }

    // TODO: Add loggers
    public async Task InvokeAsync(HttpContext httpContext)
    {
        string? apiKey = _yarpAppSecret.YarpApiKey;

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

        httpContext.Request.Headers[HeaderNames.YARP_API_KEY] = apiKey;

        await _next(httpContext);
    }
}