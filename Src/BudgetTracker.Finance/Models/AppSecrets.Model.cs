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
    
    [ConfigurationKeyName("WRITE_USERNAME")]
    public string WriteUsername { get; set; } = string.Empty;
    
    [ConfigurationKeyName("WRITE_PASSWORD")]
    public string WritePassword { get; set; } = string.Empty;
    
    [ConfigurationKeyName("WRITE_PORT")]
    public string WritePort { get; set; } = string.Empty;
    
    [ConfigurationKeyName("READ_USERNAME")]
    public string ReadUsername { get; set; } = string.Empty;
    
    [ConfigurationKeyName("READ_PASSWORD")]
    public string ReadPassword { get; set; } = string.Empty;
    
    [ConfigurationKeyName("READ_PORT")]
    public string ReadPort { get; set; } = string.Empty;
    
    [ConfigurationKeyName("MIGRATE_USERNAME")]
    public string MigrateUsername { get; set; } = string.Empty;
    
    [ConfigurationKeyName("MIGRATE_PASSWORD")]
    public string MigratePassword { get; set; } = string.Empty;
    
    [ConfigurationKeyName("MIGRATE_PORT")]
    public string MigratePort { get; set; } = string.Empty;
}

public record PubSubSecrets
{
    public string Topic { get; set; } = string.Empty;
}