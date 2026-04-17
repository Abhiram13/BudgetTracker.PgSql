using System.Net;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Exceptions;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        const string CONTENT_TYPE = "application/json";
        HttpRequest request = httpContext.Request;
        HttpResponse response = httpContext.Response;
        string traceId = request.Headers[SharedConstants.Headers.X_TRACE_ID]!;
        string requestUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

        (HttpStatusCode httpStatusCode, int apiStatusCode, string logMessage, string errorMessage) = exception switch
        {
            InvalidDateException => (HttpStatusCode.BadRequest, StatusCodes.Status400BadRequest, "Invalid Date Exception at Request = {Request} with Trace-Id = {TraceId}. Exception message = {ExceptionMessage}", "Invalid Date provided. Please check logs for more details"),
            InvalidPayloadException => (HttpStatusCode.BadRequest, StatusCodes.Status400BadRequest, "Invalid Payload Exception at Request = {Request} with Trace-Id = {TraceId}. Exception message = {ExceptionMessage}", "Invalid Payload provided. Please check logs for more details"),
            InvalidOperationException => (HttpStatusCode.Forbidden, StatusCodes.Status403Forbidden, "Invalid Authorisation attempt at Request = {Request} with Trace-Id = {TraceId}. Exception message = {ExceptionMessage}", "You are not allowed to access this resource. Please check logs from more details"), 
            DbUpdateException => (HttpStatusCode.InternalServerError, StatusCodes.Status500InternalServerError, "DB Exception at Request = {Request} with Trace-Id = {TraceId}, Exception message = {ExceptionMessage}", "Something went wrong. Please check logs for more details"),
            _ => (HttpStatusCode.InternalServerError, StatusCodes.Status500InternalServerError, "Unhandled Exception at Request = {Request} with Trace-Id = {TraceId}. Exception message = {ExceptionMessage}", "Unhandled exception occured. Please check logs for more details"),
        };
        
        _logger.LogError(exception: exception, message: logMessage, requestUrl, traceId, exception.InnerException?.Message ?? exception.Message);

        response.StatusCode = apiStatusCode;
        response.ContentType = CONTENT_TYPE;
        
        await response.WriteAsJsonAsync(new ApiResponse<string>
        {
            StatusCode = httpStatusCode,
            Message = errorMessage,
            TraceId = traceId
        });
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }
}