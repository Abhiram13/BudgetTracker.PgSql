using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BudgetTracker.Shared.Constants;
using IntegrationTests.Finance.Factory;
using IntegrationTests.Finance.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Finance.Fixtures;

public abstract class FinanceTestFixture
{
    public FinanceTestWebApplicationFactory Factory { get; } = new FinanceTestWebApplicationFactory();
    public HttpClient Client { get; }
    public HttpClient UnAuthorizedClient { get; }

    protected FinanceTestFixture()
    {
        Client = Factory.CreateClient();
        UnAuthorizedClient = Factory.CreateClient();
    }

    protected void SetClientHeaders(FinanceConfig config)
    {
        // string token = JwtTokenGenerator.CreateToken(secretKey: config.JwtSecret.Key, scope: SharedConstants.Jwt.Scopes.DOWNSTREAM, audience: config.JwtSecret.Audience);
        // Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
    }

    protected void DisposeFactoryAndClient()
    {
        Client.Dispose();
        UnAuthorizedClient.Dispose();
        Factory.Dispose();
    }
}

public static class JwtTokenGenerator
{
    public static string CreateToken(string secretKey, string scope, string audience)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Claim[] claims = new Claim[] { new Claim("scope", scope) };
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: audience,
            claims: claims, 
            expires: DateTime.UtcNow.AddMinutes(1), 
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}