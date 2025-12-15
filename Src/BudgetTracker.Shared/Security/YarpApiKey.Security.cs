using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BudgetTracker.Shared.Security;

public class YarpApiKeySchemaOptions : AuthenticationSchemeOptions
{
    public const string DefaultSchema = "ApiKeySchema";
    public const string HeaderName = "YARP_API_KEY";
}

public class YarpApiKeyHandler : AuthenticationHandler<YarpApiKeySchemaOptions>
{
    public YarpApiKeyHandler(IOptionsMonitor<YarpApiKeySchemaOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        bool isHeaderExist = Request.Headers.ContainsKey(YarpApiKeySchemaOptions.HeaderName);

        if (!isHeaderExist)
        {
            return Task.FromResult(AuthenticateResult.Fail("No Key"));
        }

        string? HEADER_API_KEY = Request.Headers[YarpApiKeySchemaOptions.HeaderName];
        string? API_KEY = Environment.GetEnvironmentVariable("YARP_API_KEY");

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
}