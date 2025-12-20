using System.Security.Claims;
using System.Text.Encodings.Web;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BudgetTracker.Gateway.Security;

public class ApiKeySchemaOptions : AuthenticationSchemeOptions
{
    public const string DefaultSchema = "ApiKeySchema";
    public const string HeaderName = "API_KEY";
}

public class ApiKeyHandler : AuthenticationHandler<ApiKeySchemaOptions>
{
    public ApiKeyHandler(IOptionsMonitor<ApiKeySchemaOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        bool isHeaderExist = Request.Headers.ContainsKey(ApiKeySchemaOptions.HeaderName);

        if (!isHeaderExist)
        {
            return Task.FromResult(AuthenticateResult.Fail("No Key"));
        }

        string? HEADER_API_KEY = Request.Headers[ApiKeySchemaOptions.HeaderName];
        string? API_KEY = Environment.GetEnvironmentVariable("API_KEY"); // TODO: Change to GCP Secrets

        if (HEADER_API_KEY != API_KEY)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid key provided"));
        }

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, "Api User")
        };

        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";

        string traceId = Request.Headers["X-Trace-Id"]!; // FIX: Default Trace ID should be generated incase none from headers

        ApiResponse<string> response = new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Unauthorized,
            TraceId = traceId,
            Message = "Unauthorised"
        };

        await Response.WriteAsJsonAsync(response);
    }
}