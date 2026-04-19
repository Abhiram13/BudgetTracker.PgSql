using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Finance.Models;

public record Secrets
{
    [ConfigurationKeyName(SharedConstants.Headers.YARP_API_KEY)]
    public required string YarpApiKey { get; init; }
}

public record PubSub
{
    public required string Topic { get; init; }
}

public record FinanceConfig : SharedSecrets
{
    [ConfigurationKeyName("SECRETS")]
    public required Secrets Secrets { get; init; }
    
    [ConfigurationKeyName("PUB_SUB")]
    public required PubSub PubSub { get; init; }
    
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public string GoogleCloudProjectId { get; init; } = string.Empty;
}