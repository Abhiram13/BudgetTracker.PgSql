using BudgetTracker.Shared.Constants;
using Microsoft.Extensions.Configuration;

namespace Warehouse.IntegrationTests.Model;

public record GoogleCloudProject
{
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public string Id { get; init; } = string.Empty;
}