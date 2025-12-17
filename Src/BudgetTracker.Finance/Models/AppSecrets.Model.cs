using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Finance.Interfaces;

namespace BudgetTracker.Finance.Models;

public sealed class AppSecrets : IYarpApiKeyAppSecret, IFinanceAppSecrets
{
    public string PostgresHost { get; set; } = string.Empty;
    public string PostgresDatabase { get; set; } = string.Empty;
    public string PostgresUsername { get; set; } = string.Empty;
    public string PostgresPassword { get; set; } = string.Empty;
    public string PostgresPort { get; set; } = string.Empty;
    public string YarpApiKey { get; set; } = string.Empty;
    public string GoogleCloudProjectId { get; set; } = string.Empty;
    public string PubSubTopic { get; set; } = string.Empty;
}