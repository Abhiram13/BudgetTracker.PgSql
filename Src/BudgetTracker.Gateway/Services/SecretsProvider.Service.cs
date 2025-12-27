using System.Threading.Tasks;
using Abhiram.Secrets.Providers;
using Abhiram.Secrets.Providers.Interface;

namespace BudgetTracker.Gateway.Services;

public class GatewayAppSecretsProvider : ConfigurationProvider
{
    private readonly ISecretManager _secretManager;

    public GatewayAppSecretsProvider(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public override void Load()
    {   
        Set("Api:ApiKey", "API_KEY");
        Set("Yarp:YarpApiKey", "YARP_API_KEY");
        Set("ReverseProxy:Clusters:finance-cluster:Destinations:instance-01:Address", "FINANCE_SERVICE_URL");
        Set("ReverseProxy:Clusters:warehouse-cluster:Destinations:instance-01:Address", "WAREHOUSE_SERVICE_URL");
        Set("ReverseProxy:Clusters:dues-cluster:Destinations:instance-01:Address", "DUES_SERVICE_URL");
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

public sealed class GatewayAppSecretsSource : IConfigurationSource
{
    private readonly ISecretManager _secretManager;

    public GatewayAppSecretsSource(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new GatewayAppSecretsProvider(_secretManager);
    }
}