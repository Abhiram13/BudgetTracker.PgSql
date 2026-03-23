using Microsoft.Extensions.Configuration;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Shared.Models;

public record YarpApiKeySecret
{
    [ConfigurationKeyName(HeaderNames.YARP_API_KEY)]
    public string YarpApiKey { get; init; } = string.Empty;
}