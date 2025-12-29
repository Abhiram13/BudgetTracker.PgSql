using System.Threading.Tasks;
using Abhiram.Secrets.Providers;
using Abhiram.Secrets.Providers.Interface;

namespace BudgetTracker.Dues.Services;

public class DuesAppSecretsProvider : ConfigurationProvider
{
    private readonly ISecretManager _secretManager;

    public DuesAppSecretsProvider(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public override void Load()
    {
        Set("Postgres:PostgresHost", "POSTGRES_HOST");
        Set("Postgres:PostgresDatabase", "DUES_POSTGRES_DATABASE");
        Set("Postgres:PostgresUsername", "POSTGRES_USERNAME");
        Set("Postgres:PostgresPassword", "POSTGRES_PASSWORD");
        Set("Postgres:PostgresPort", "POSTGRES_WRITE_PORT");
        Set("Yarp:YarpApiKey", "YARP_API_KEY");
    }

    private new void Set(string configKey, string secretName)
    {
        string value = _secretManager.GetSecretAsync(secretName).ConfigureAwait(false).GetAwaiter().GetResult();

        if (!string.IsNullOrEmpty(value))
        {
            Data[configKey] = value;
        }
    }
}

public sealed class DuesAppSecretsSource : IConfigurationSource
{
    private readonly ISecretManager _secretManager;

    public DuesAppSecretsSource(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DuesAppSecretsProvider(_secretManager);
    }
}