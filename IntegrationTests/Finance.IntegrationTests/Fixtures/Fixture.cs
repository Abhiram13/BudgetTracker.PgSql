using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BudgetTracker.Finance;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Security;
using IntegrationTests.Finance.Factory;
using IntegrationTests.Finance.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// since hard-coded banks, dues, category ids "1" and "2" are used in transaction tests, resetting the banks, dues, category table identity.
    /// If not every transactions tests access new dynamic bank or category id
    /// </summary>
    /// <param name="dbContext"></param>
    protected async Task TruncateTables(WriteDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE categories RESTART IDENTITY CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE banks RESTART IDENTITY CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE dues RESTART IDENTITY CASCADE");
    }
}