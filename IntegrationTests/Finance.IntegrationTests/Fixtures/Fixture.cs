using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Security;
using IntegrationTests.Finance.Factory;
using IntegrationTests.Finance.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Finance.Fixtures;

public abstract class FinanceTestFixture
{
    public FinanceTestWebApplicationFactory Factory { get; }
    public HttpClient Client { get; }
    public HttpClient UnAuthorizedClient { get; }

    protected FinanceTestFixture(FinanceTestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
        UnAuthorizedClient = Factory.CreateClient();
    }

    protected void SetClientHeaders(JwtConfiguration config)
    {
        string token = JwtFactory.CreateToken(configuration: config, scope: SharedConstants.Jwt.Scopes.DOWNSTREAM);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
    }

    protected void DisposeClients()
    {
        Client.Dispose();
        UnAuthorizedClient.Dispose();
    }
}