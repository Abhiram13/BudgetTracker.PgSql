using BudgetTracker.Shared.Constants;
using Microsoft.Extensions.Configuration;

namespace Finance.IntegrationTests.Models;

public record DatabaseConnection
{
    public required string FinanceDb { get; init; }
}

public record Secrets
{
    [ConfigurationKeyName(HeaderNames.YARP_API_KEY)]
    public required string YarpApiKey { get; init; }
}

public record PubSub
{
    public required string Topic { get; init; }
}

public record FinanceConfig
{
    [ConfigurationKeyName("DbConnectionStrings")]
    public required DatabaseConnection DatabaseConnection { get; init; }
    
    [ConfigurationKeyName("SECRETS")]
    public required Secrets Secrets { get; init; }
    
    [ConfigurationKeyName("PUB_SUB")]
    public required PubSub PubSub { get; init; }
}