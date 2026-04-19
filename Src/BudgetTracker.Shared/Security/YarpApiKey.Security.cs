using System.Security.Claims;
using System.Text.Encodings.Web;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BudgetTracker.Shared.Security;

public class YarpApiKeySchemaOptions : AuthenticationSchemeOptions
{
    public const string DefaultSchema = "YarpApiKeySchema";
    public const string HeaderName = SharedConstants.Headers.YARP_API_KEY;
}

// Used to verify and authenticate client api calls if YARP_API_KEY exists in header
// Used in downstream apis
[Obsolete(message: "Use JWT for auth", error: true)]
public class YarpApiKeyHandler : AuthenticationHandler<YarpApiKeySchemaOptions>
{
    // private readonly YarpApiKeySecret _appSecrets;
    
    public YarpApiKeyHandler(IOptionsMonitor<YarpApiKeySchemaOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        bool isHeaderExist = Request.Headers.ContainsKey(YarpApiKeySchemaOptions.HeaderName);

        if (!isHeaderExist)
        {
            return Task.FromResult(AuthenticateResult.Fail("No Key"));
        }

        string? HEADER_API_KEY = Request.Headers[YarpApiKeySchemaOptions.HeaderName];
        string? API_KEY = "";

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

        string traceId = Request.Headers[SharedConstants.Headers.X_TRACE_ID]!; // FIX: Default Trace ID should be generated incase none from headers

        ApiResponse<string> response = new ApiResponse<string>
        {
            StatusCode = System.Net.HttpStatusCode.Unauthorized,
            TraceId = traceId, // TODO: Trace Id is null here.
            Message = "Unauthorised"
        };

        await Response.WriteAsJsonAsync(response);
    }
}