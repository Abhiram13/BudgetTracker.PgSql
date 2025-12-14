namespace BudgetTracker.Gateway.Interfaces;

public interface ICustomMiddleware
{
    Task InvokeAsync(HttpContext httpContext);
}