using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Gateway.Models;

public record Secrets : YarpApiKeySecret
{
    [ConfigurationKeyName("API_KEY")]
    public string ApiKey { get; set; } = string.Empty;
}

public record GatewayAppSecrets
{
    public Secrets Secrets { get; set; } = default!;
}