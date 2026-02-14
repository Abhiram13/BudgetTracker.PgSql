namespace IntegrationTests;

public class IntegrationTestFixture : IAsyncLifetime
{
    private FinanceTestWebApplicationFactory? _factory { get; set; }
    public HttpClient? Client { get; private set; }
    
    public Task InitializeAsync()
    {
        _factory = new FinanceTestWebApplicationFactory();
        Client = _factory.CreateClient();
        
        SetClientHeaders();
        
        return Task.CompletedTask;
    }

    private void SetClientHeaders()
    {
        string traceId = Guid.NewGuid().ToString();
        string? YARPAPIKEY = Environment.GetEnvironmentVariable("YARP_API_KEY");

        if (Client is null) return;
        
        Client.DefaultRequestHeaders.Add("X-Trace-Id", traceId);
            
        if (YARPAPIKEY is not null)
        {
            Client.DefaultRequestHeaders.Add("YARP_API_KEY", YARPAPIKEY);
        }
    }

    public Task DisposeAsync()
    {
        Client?.Dispose();
        _factory?.Dispose();
        
        return Task.CompletedTask;
    }
}

public abstract class BaseIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    protected readonly HttpClient _client;
    protected readonly IntegrationTestFixture _fixture;

    public BaseIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client!;
    }
}