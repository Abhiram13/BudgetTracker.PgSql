using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Dues.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Dues.Models;

public sealed record DueAppSecrets
{
    public PostgresSecrets Postgres { get; set; } = default!;
}

public record PostgresSecrets
{
    [ConfigurationKeyName("DUES_DATABASE")]
    public string Database { get; init; } = string.Empty;
    public string Host { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Port { get; init; } = string.Empty;
}