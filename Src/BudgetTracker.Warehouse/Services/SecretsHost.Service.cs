using Abhiram.Secrets.Providers.Interface;
using BudgetTracker.Warehouse.Interfaces;
using BudgetTracker.Shared.Interfaces;

namespace BudgetTracker.Warehouse.Services;

/// <summary>
/// A hosted service responsible for retrieving secrets at application startup
/// and populating the <see cref="AppSecrets"/> singleton with those values.
/// </summary>
public class SecretHostService : IHostedService
{
    private readonly ISecretManager _secretManager;
    private readonly IWarehouseAppSecrets _appSecrets;
    private readonly IYarpApiKeyAppSecret _yarpSecretKey;

    // TODO: Update Summary below
    /// <summary>
    /// A hosted service responsible for retrieving secrets at application startup
    /// and populating the <see cref="AppSecrets"/> singleton with those values. <br />
    /// Initializes a new instance of the <see cref="SecretHostService"/> class.
    /// </summary>
    /// <param name="secretManager">The service used to retrieve secrets asynchronously.</param>
    /// <param name="appSecrets">The singleton instance of <see cref="AppSecrets"/> to populate with secret values.</param>
    public SecretHostService(ISecretManager secretManager, IWarehouseAppSecrets appSecrets, IYarpApiKeyAppSecret yarpApiKeySecret)
    {
        _appSecrets = appSecrets;
        _secretManager = secretManager;
        _yarpSecretKey = yarpApiKeySecret;
    }

    /// <summary>
    /// Called by the host when the application is starting.
    /// Retrieves secret values asynchronously and populates the <see cref="AppSecrets"/> instance.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _appSecrets.GoogleProjectId = await _secretManager.GetSecretAsync("GOOGLE_CLOUD_PROJECT_ID");
        _appSecrets.DatewiseTransactionSubscriber = await _secretManager.GetSecretAsync("PUBSUB_DATEWISE_TRANSACTIONS_SUBSCRIBER");
        _appSecrets.DataSet = await _secretManager.GetSecretAsync("DATASET");
        _yarpSecretKey.YarpApiKey = await _secretManager.GetSecretAsync("YARP_API_KEY");
    }

    /// <summary>
    /// Called by the host when the application is shutting down.
    /// This implementation does nothing.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}