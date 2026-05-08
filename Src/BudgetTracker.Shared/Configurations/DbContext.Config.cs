using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BudgetTracker.Shared.Configurations;

/// <summary>
/// Holds Db credentials like host, username, password, port and database
/// </summary>
/// <remarks>Common DTO for all DB types (Write, Read and Migrate)</remarks>
public record DatabaseConfiguration
{
    /// <summary>
    /// Host of the DB (Write, Read or Migrate)
    /// </summary>
    [ConfigurationKeyName("Host")]
    public string Host { get; init; } = string.Empty;
    
    /// <summary>
    /// Port of the DB (Write, Read or Migrate)
    /// </summary>
    [ConfigurationKeyName("Port")]
    public string Port { get; init; } = string.Empty;
    
    /// <summary>
    /// Name of the DB (Write, Read or Migrate)
    /// </summary>
    [ConfigurationKeyName("Database")]
    public string Database { get; init; } = string.Empty;
    
    /// <summary>
    /// Username of the DB (Write, Read or Migrate)
    /// </summary>
    [ConfigurationKeyName("Username")]
    public string Username { get; init; } = string.Empty;
    
    /// <summary>
    /// Password of the DB (Write, Read or Migrate)
    /// </summary>
    [ConfigurationKeyName("Password")]
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Contains <c>WRITE</c>, <c>READ</c>, <c>MIGRATE</c>
/// </summary>
public static class DatabaseType
{
    public const string WRITE = "WRITE";
    public const string READ = "READ";
    public const string MIGRATE = "MIGRATE";
}

public class DatabaseConfigurationSetup : IConfigureNamedOptions<DatabaseConfiguration>
{
    private readonly IConfiguration _configuration;

    public DatabaseConfigurationSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(DatabaseConfiguration options)
    {
        // By default, setting 'WRITE'
        Configure(name: DatabaseType.WRITE, options);
    }

    public void Configure(string? name, DatabaseConfiguration options)
    {
        _configuration.GetSection($"Postgres:{name}").Bind(options);
    }
}