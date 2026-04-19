using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Gateway.Models;

public record GatewayAppSecrets
{
    [ConfigurationKeyName("API_KEY")]
    public string ApiKey { get; set; } = string.Empty;
}