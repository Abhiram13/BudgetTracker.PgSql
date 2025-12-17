using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Dues.Interfaces;

namespace BudgetTracker.Dues.Models;

public sealed class DueAppSecrets : IYarpApiKeyAppSecret, IDueAppSecrets
{
    public string PostgresHost { get; set; } = string.Empty;
    public string PostgresDatabase { get; set; } = string.Empty;
    public string PostgresUsername { get; set; } = string.Empty;
    public string PostgresPassword { get; set; } = string.Empty;
    public string PostgresPort { get; set; } = string.Empty;
    public string YarpApiKey { get; set; } = string.Empty;
}