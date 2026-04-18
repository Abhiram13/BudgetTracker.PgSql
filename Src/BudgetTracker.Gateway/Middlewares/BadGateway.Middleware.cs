using System.Net;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using Yarp.ReverseProxy.Forwarder;

namespace BudgetTracker.Gateway.Middlewares;

/// <summary>
/// Middleware used to send <c>502</c> Bad gateway response if downstream apis are down or not available 
/// </summary>
public class BadGatewayMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;

    public BadGatewayMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;
    }

    // TODO: Add loggers
    public async Task InvokeAsync(HttpContext httpContext)
    {
        await _next(httpContext);

        IForwarderErrorFeature? proxyError = httpContext.Features.Get<IForwarderErrorFeature>();
        if (proxyError is null) return;

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = StatusCodes.Status502BadGateway;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(new ApiResponse<string>
        {
            StatusCode = HttpStatusCode.BadGateway,
            Message = "Downstream service unavailable",
            TraceId = "" // TODO: Get trace ID here
        });
    }
}