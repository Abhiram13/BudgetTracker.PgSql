using BudgetTracker.Gateway.Interfaces;
using Yarp.ReverseProxy.Forwarder;

namespace BudgetTracker.Gateway.Middlewares;

public class BadGatewayMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _next;

    public BadGatewayMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        await _next(httpContext);

        IForwarderErrorFeature? proxyError = httpContext.Features.Get<IForwarderErrorFeature>();
        if (proxyError is null) return;

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = StatusCodes.Status502BadGateway;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(new
        {
            Error = "Bad Gateway",
            Message = "Downstream service unavailable"
        });
    }
}