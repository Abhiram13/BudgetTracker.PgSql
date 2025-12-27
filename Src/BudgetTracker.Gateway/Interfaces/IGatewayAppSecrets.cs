using BudgetTracker.Shared.Interfaces;

namespace BudgetTracker.Gateway.Interfaces;

public interface IGatewayAppSecrets
{
    string ApiKey { get; set; }
}