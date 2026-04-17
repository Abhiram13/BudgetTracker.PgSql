using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Shared.Models;

/// <summary>
/// Maps <c>JWT</c> property from <c>appsettings.{env}.json</c> or from Environmental variables
/// </summary>
public record JwtSecret
{
    /// <summary>
    /// Maps <c>SECURITY_KEY</c> property from <c>JWT</c>
    /// </summary>
    [ConfigurationKeyName("SECURITY_KEY")]
    public string Key { get; init; } = string.Empty;
    
    /// <summary>
    /// Maps <c>Audience</c> property from <c>JWT</c>
    /// </summary>
    public string Audience { get; init; } = string.Empty;
}