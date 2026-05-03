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
public sealed record AppSecrets
{
    /// <summary>
    /// Gets the configuration settings for Google Cloud Pub/Sub.
    /// </summary>
    [ConfigurationKeyName("PubSub")]
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
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public string GoogleCloudProjectId { get; init; } = string.Empty;
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