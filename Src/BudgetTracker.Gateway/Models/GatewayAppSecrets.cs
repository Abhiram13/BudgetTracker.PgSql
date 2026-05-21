using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Gateway.Models;

public record GatewayAppSecrets
{
    [ConfigurationKeyName("ApiKey")]
    public string ApiKey { get; set; } = string.Empty;
}