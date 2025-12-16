using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Shared.Middlwares;

public class ExceptionHandlerMiddleware : ICustomMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _requestDelegate = requestDelegate;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (Exception exception)
        {
            string traceId = httpContext.Request.Headers["X-Trace-Id"]!;
            HttpRequest request = httpContext.Request;
            string requestUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            _logger.LogError(exception, message: "Unhandled Exception at Request = {0} with Trace-Id = {1}. Exception message = {2}", requestUrl, traceId, exception.Message);
            
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new ApiResponse<string>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                TraceId = traceId,
                Message = "Unhandled exception occured. Please check logs for more details"
            });
        }
    }
}