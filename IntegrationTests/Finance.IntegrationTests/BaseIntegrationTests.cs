using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using IntegrationTests.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IntegrationTests.Exceptions;

namespace IntegrationTests;

public class IntegrationTestFixture : IAsyncLifetime
{
    private FinanceTestWebApplicationFactory? _factory { get; set; }
    public HttpClient? Client { get; private set; }
    public Category _testCategory; 
    
    public async Task InitializeAsync()
    {
        _factory = new FinanceTestWebApplicationFactory();
        Client = _factory.CreateClient();
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        // optional but recommended
        await context.Database.EnsureCreatedAsync();

        _testCategory = await new CategoryBuilder(context).CreateCategoryAsync();
        
        SetClientHeaders();

        return;
    }

    private void SetClientHeaders()
    {
        string traceId = Guid.NewGuid().ToString();
        string? yarpApiKey;

        if (_factory == null) throw new NotFoundException($"FinanceWebApplicationFactory ({_factory}) not found");
        
        using (IServiceScope scope = _factory.CreateScope())
        {
            IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            yarpApiKey = configuration.GetSection("SECRETS")["YARP_API_KEY"];
        }
        
        if (yarpApiKey == null) throw new NotFoundException($"YARP_API_KEY ({yarpApiKey}) is missing");
        
        if (Client == null) throw new NotFoundException($"Client ({Client}) is missing");
        
        Client.DefaultRequestHeaders.Add("X-Trace-Id", traceId);
        Client.DefaultRequestHeaders.Add("YARP_API_KEY", yarpApiKey);
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