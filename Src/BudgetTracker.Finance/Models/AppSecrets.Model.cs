using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Models;

public sealed class AppSecrets
{
    [ConfigurationKeyName("PUB_SUB")]
    public PubSubSecrets PubSub { get; set; } = default!;
    public PostgresSecrets Postgres { get; set; } = default!;
    public YarpApiKeySecret Secrets { get; set; } = default!;
}

public record PostgresSecrets
{
    [ConfigurationKeyName("FINANCE_DATABASE")]
    public string Database { get; set; } = string.Empty;
    public string Host { get; set; }= string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
}

public record PubSubSecrets
{
    public string Topic { get; set; } = string.Empty;
}