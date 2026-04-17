using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Shared.Models;

public record JwtSecret
{
    [ConfigurationKeyName("SECURITY_KEY")]
    public string Key { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
}