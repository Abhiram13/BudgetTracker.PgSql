using System.ComponentModel.DataAnnotations;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Configurations;

/// <summary>
/// Holds secrets or configuration required for the application
/// </summary>
/// <remarks>
/// This record is updated from multiple configuration sources including appsettings.json, environment variables, and secret managers.
/// </remarks>
public sealed record AppSecrets
{
    /// <summary>
    /// Gets the configuration settings for Google Cloud Pub/Sub.
    /// </summary>
    public PubSubSecrets PubSub { get; init; } = default!;

    /// <summary>
    /// Gets the settings for Transactional Outbox pattern, such as period time interval for outbox background service.
    /// </summary>
    [ConfigurationKeyName("OutBox")]
    public OutboxConfig OutboxConfig { get; init; } = default!;
    
    /// <summary>
    /// Gets the ID of Google Cloud project.
    /// </summary>
    /// <remarks>
    /// Typically mapped from the <c>GOOGLE_CLOUD_PROJECT_ID</c> environment variable.
    /// </remarks>
    [Required]
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public string GoogleCloudProjectId { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets or Sets the port of the current App server through environmental variable <c>PORT</c>
    /// </summary>
    [Required]
    [ConfigurationKeyName("PORT")]
    public int ServerPort { get; init; }

    [Required]
    [ConfigurationKeyName("Postgres")]
    public required DatabaseConfiguration PostgresDatabase { get; init; }
}

/// <summary>
/// Holds config like Topic or Subscription names related to Google PubSub
/// </summary>
public record PubSubSecrets
{
    /// <summary>
    /// PubSub topic name
    /// </summary>
    [Required]
    public string Topic { get; init; } = string.Empty;
    
    /// <summary>
    /// PubSub subscriber name
    /// </summary>
    [Required]
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
    /// <value>In Seconds</value>
    public int Period { get; init; } = 100;
}

/// <summary>
/// Holds Db credentials like host, username, password, port and database
/// </summary>
/// <remarks>Common DTO for all DB types (Write, Read and Migrate)</remarks>
public record DatabaseConfiguration
{
    /// <summary>
    /// Host of the DB (Write, Read or Migrate)
    /// </summary>
    public string Host { get; init; } = string.Empty;
    
    /// <summary>
    /// Port of the DB (Write, Read or Migrate)
    /// </summary>
    public string Port { get; init; } = string.Empty;
    
    /// <summary>
    /// Name of the DB (Write, Read or Migrate)
    /// </summary>
    public string Database { get; init; } = string.Empty;
    
    /// <summary>
    /// Username of the DB (Write, Read or Migrate)
    /// </summary>
    public string Username { get; init; } = string.Empty;
    
    /// <summary>
    /// Password of the DB (Write, Read or Migrate)
    /// </summary>
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Contains <c>WRITE</c>, <c>READ</c>, <c>MIGRATE</c>
/// </summary>
public static class DatabaseType
{
    public const string WRITE = "Write";
    public const string READ = "Read";
    public const string MIGRATE = "Migrate";
}