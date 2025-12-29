using System.Threading.Tasks;
using Abhiram.Secrets.Providers;
using Abhiram.Secrets.Providers.Interface;

namespace BudgetTracker.Warehouse.Services;

public class WarehouseAppSecretsProvider : ConfigurationProvider
{
    private readonly ISecretManager _secretManager;

    public WarehouseAppSecretsProvider(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public override void Load()
    {
        Set("PubSub:DatewiseTransactionSubscriber", "PUBSUB_DATEWISE_TRANSACTIONS_SUBSCRIBER");
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

public sealed class WarehouseAppSecretsSource : IConfigurationSource
{
    private readonly ISecretManager _secretManager;

    public WarehouseAppSecretsSource(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new WarehouseAppSecretsProvider(_secretManager);
    }
}