using BudgetTracker.Shared.Constants;
using IntegrationTests.Finance.Factory;
using IntegrationTests.Finance.Models;

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
        string traceId = Guid.NewGuid().ToString();
        string yarpApiKey = config.Secrets.YarpApiKey;
        
        Client.DefaultRequestHeaders.Add(SharedConstants.Headers.YARP_API_KEY, yarpApiKey);
        Client.DefaultRequestHeaders.Add(SharedConstants.Headers.X_TRACE_ID, traceId);
    }

    protected void DisposeFactoryAndClient()
    {
        Client.Dispose();
        UnAuthorizedClient.Dispose();
        Factory.Dispose();
    }
}