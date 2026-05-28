using Microsoft.Extensions.Options;

namespace BudgetTracker.Finance.Configurations;

/// <summary>
/// In this setup <c>Postgres</c> section would be pulled from configuration sources (.env, appsettings(.env).json, secret managers) and bind those config into <see cref="DatabaseConfiguration"/>
/// </summary>
/// <remarks>
/// <para>This setup inherits from <see cref="IConfigureNamedOptions{TOptions}"/> where <c>TOptions</c> is <see cref="DatabaseConfiguration"/></para>
/// </remarks>
public class DatabaseConfigurationSetup : IConfigureNamedOptions<DatabaseConfiguration>
{
    private readonly IConfiguration _configuration;

    /// <inheritdoc cref="DatabaseConfigurationSetup"/>
    /// <param name="configuration">To get <c>Postgres</c> section from configuration sources.</param>
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

