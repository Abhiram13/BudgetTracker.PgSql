using Microsoft.AspNetCore.Http;

namespace BudgetTracker.Shared.Interfaces;

public interface ICustomMiddleware
{
    Task InvokeAsync(HttpContext httpContext);
}