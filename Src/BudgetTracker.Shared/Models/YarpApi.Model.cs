using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Shared.Models;

public record YarpApiKeySecret
{
    [ConfigurationKeyName("YARP_API_KEY")]
    public string YarpApiKey { get; init; } = string.Empty;
}