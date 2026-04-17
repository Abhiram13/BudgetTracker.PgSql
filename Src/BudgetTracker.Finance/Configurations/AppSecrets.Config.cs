using System.ComponentModel.DataAnnotations;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Configurations;

/// <summary>
/// Represents the strongly-typed root configuration for the application.
/// </summary>
/// <remarks>
/// This record is bound from multiple configuration sources including appsettings.json, environment variables, and secret managers.
/// </remarks>
public sealed record AppSecrets : SharedSecrets
{
    /// <summary>
    /// Gets the configuration settings for Google Cloud Pub/Sub.
    /// </summary>
    [ConfigurationKeyName("PUB_SUB")]
    public PubSubSecrets PubSub { get; init; } = default!;

    /// <summary>
    /// Gets the settings for Transactional Outbox pattern, such as period time interval for outbox background service.
    /// </summary>
    [ConfigurationKeyName("OutBox")]
    public OutboxConfig OutboxConfig { get; init; } = default!;
    
    /// <summary>
    /// Gets the credentials and connection parameters for the PostgreSQL database.
    /// </summary>
    [ConfigurationKeyName("POSTGRES")]
    public PostgresSecrets Postgres { get; init; } = default!;
    
    // [ConfigurationKeyName("SECRETS")]
    // public YarpApiKeySecret Secrets { get; init; } = default!;

    // [ConfigurationKeyName("JWT")]
    // public JwtSecret JwtSecret { get; init; } = default!;
    
    /// <summary>
    /// Gets the ID of Google Cloud project.
    /// </summary>
    /// <remarks>
    /// Typically mapped from the <c>GOOGLE_CLOUD_PROJECT_ID</c> environment variable.
    /// </remarks>
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public string GoogleCloudProjectId { get; init; } = string.Empty;
}

/// <summary>
/// Contains credentials related to Postgres write, read and migrate DBs, hosts
/// </summary>
public record PostgresSecrets
{
    /// <summary>
    /// Holds DB name
    /// </summary>
    [Required(ErrorMessage = "Finance DB is missing or invalid")]
    [ConfigurationKeyName("FINANCE_DATABASE")]
    public string Database { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds DB host
    /// </summary>
    [Required(ErrorMessage = "DB Host is missing or invalid")]
    public string Host { get; init; }= string.Empty;
    
    /// <summary>
    /// Holds username of Write role
    /// </summary>
    [Required(ErrorMessage = "Primary user name is missing or invalid")]
    [ConfigurationKeyName("WRITE_USERNAME")]
    public string WriteUsername { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds password of Write role
    /// </summary>
    [Required(ErrorMessage = "Primary user password is missing or invalid")]
    [ConfigurationKeyName("WRITE_PASSWORD")]
    public string WritePassword { get; init; } = string.Empty;
    
    
    /// <summary>
    /// Holds port of Write role
    /// </summary>
    [Required(ErrorMessage = "Primary DB port is missing or invalid")]
    [ConfigurationKeyName("WRITE_PORT")]
    public string WritePort { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds username of Read role
    /// </summary>
    [Required(ErrorMessage = "Replica user name is missing or invalid")]
    [ConfigurationKeyName("READ_USERNAME")]
    public string ReadUsername { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds password of Read role
    /// </summary>
    [Required(ErrorMessage = "Replica user password is missing or invalid")]
    [ConfigurationKeyName("READ_PASSWORD")]
    public string ReadPassword { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds port of Read role
    /// </summary>
    [Required(ErrorMessage = "Replica DB port is missing or invalid")]
    [ConfigurationKeyName("READ_PORT")]
    public string ReadPort { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds username of Migrate role
    /// </summary>
    [Required(ErrorMessage = "Migrate user name is missing or invalid")]
    [ConfigurationKeyName("MIGRATE_USERNAME")]
    public string MigrateUsername { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds password of Migrate role
    /// </summary>
    [Required(ErrorMessage = "Migrate user password is missing or invalid")]
    [ConfigurationKeyName("MIGRATE_PASSWORD")]
    public string MigratePassword { get; init; } = string.Empty;
    
    /// <summary>
    /// Holds port of Migrate role
    /// </summary>
    [Required(ErrorMessage = "Migrate DB port is missing or invalid")]
    [ConfigurationKeyName("MIGRATE_PORT")]
    public string MigratePort { get; init; } = string.Empty;
}

/// <summary>
/// Holds config like Topic or Subscription names related to Google PubSub
/// </summary>
public record PubSubSecrets
{
    /// <summary>
    /// PubSub topic name
    /// </summary>
    public string Topic { get; init; } = string.Empty;
    
    /// <summary>
    /// PubSub subscriber name
    /// </summary>
    public string Subscriber { get; init; } = string.Empty;
}

/// <summary>
/// Holds general config of Outbox events
/// </summary>
public record OutboxConfig
{
    /// <summary>
    /// Interval time to run Outbox background service
    /// </summary>
    public int Period { get; init; } = 100;
}