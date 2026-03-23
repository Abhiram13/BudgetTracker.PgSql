using System.Security.Claims;
using System.Text.Encodings.Web;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BudgetTracker.Shared.Security;

public class YarpApiKeySchemaOptions : AuthenticationSchemeOptions
{
    public const string DefaultSchema = "YarpApiKeySchema";
    public const string HeaderName = HeaderNames.YARP_API_KEY;
}

public class YarpApiKeyHandler : AuthenticationHandler<YarpApiKeySchemaOptions>
{
    private readonly YarpApiKeySecret _appSecrets;

    public YarpApiKeyHandler(IOptionsMonitor<YarpApiKeySchemaOptions> options, ILoggerFactory logger, UrlEncoder encoder, YarpApiKeySecret appSecrets) : base(options, logger, encoder)
    {
        _appSecrets = appSecrets;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        bool isHeaderExist = Request.Headers.ContainsKey(YarpApiKeySchemaOptions.HeaderName);

        if (!isHeaderExist)
        {
            return Task.FromResult(AuthenticateResult.Fail("No Key"));
        }

        string? HEADER_API_KEY = Request.Headers[YarpApiKeySchemaOptions.HeaderName];
        string? API_KEY = _appSecrets.YarpApiKey;

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