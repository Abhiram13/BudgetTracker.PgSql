using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Finance.Models;

public record PubSub
{
    public required string Topic { get; init; }
}

public record FinanceConfig
{
    [ConfigurationKeyName("PUB_SUB")]
    public required PubSub PubSub { get; init; }
    
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public string GoogleCloudProjectId { get; init; } = string.Empty;
}