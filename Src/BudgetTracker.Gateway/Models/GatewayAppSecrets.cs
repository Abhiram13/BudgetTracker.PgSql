using BudgetTracker.Gateway.Interfaces;
using BudgetTracker.Shared.Interfaces;

namespace BudgetTracker.Gateway.Models;

public class GatewayAppSecrets : IYarpApiKeyAppSecret, IGatewayAppSecrets
{
    public string YarpApiKey { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}