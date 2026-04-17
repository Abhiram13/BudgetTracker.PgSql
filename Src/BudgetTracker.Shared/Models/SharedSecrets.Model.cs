using BudgetTracker.Shared.Constants;
using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Shared.Models;

public record SharedSecrets
{
    [Obsolete("Use JWT")]
    [ConfigurationKeyName(SharedConstants.Headers.YARP_API_KEY)]
    public string YarpApiKey { get; init; } = string.Empty;
    
    [ConfigurationKeyName("JWT")]
    public JwtSecret JwtSecret { get; init; } = default!;
}