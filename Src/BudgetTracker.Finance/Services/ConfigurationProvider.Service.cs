// using System.Threading.Tasks;
// using Abhiram.Secrets.Providers;
// using Abhiram.Secrets.Providers.Interface;

// namespace BudgetTracker.Finance.Services;

// [Obsolete(message: "Directly use .AddSecrets() method in WebApplication from Abhiram.Secrets.Configuration namespace", error: true)]
// public class FinanceAppSecretsProvider : ConfigurationProvider
// {
//     private readonly ISecretManager _secretManager;

//     public FinanceAppSecretsProvider(ISecretManager secretManager)
//     {
//         _secretManager = secretManager;
//     }

//     public override void Load()
//     {
//         Set("Postgres:PostgresHost", "POSTGRES_HOST");
//         Set("Postgres:PostgresPort", "POSTGRES_WRITE_PORT");
//         Set("Postgres:PostgresDatabase", "FINANCE_POSTGRES_DATABASE");
//         Set("Postgres:PostgresUsername", "POSTGRES_USERNAME");
//         Set("Postgres:PostgresPassword", "POSTGRES_PASSWORD");
//         Set("Postgres:GoogleCloudProjectId", "GOOGLE_CLOUD_PROJECT_ID");
//         Set("Postgres:PubSubTopic", "PUB_SUB_TOPIC");
//         Set("Yarp:YarpApiKey", "YARP_API_KEY");
//     }

//     private new void Set(string configKey, string secretName)
//     {
//         string value = _secretManager.GetSecretAsync(secretName).ConfigureAwait(false).GetAwaiter().GetResult();

//         if (!string.IsNullOrEmpty(value))
//         {
//             Data[configKey] = value;
//         }
//     }
// }

// [Obsolete(message: "Directly use .AddSecrets() method in WebApplication from Abhiram.Secrets.Configuration namespace", error: true)]
// public sealed class FinanceAppSecretsSource : IConfigurationSource
// {
//     private readonly ISecretManager _secretManager;

//     public FinanceAppSecretsSource(ISecretManager secretManager)
//     {
//         _secretManager = secretManager;
//     }

//     public IConfigurationProvider Build(IConfigurationBuilder builder)
//     {
//         return new FinanceAppSecretsProvider(_secretManager);
//     }
// }