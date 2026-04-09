using Abhiram.Secrets.Providers.Exceptions;
using Google.Cloud.PubSub.V1;

namespace BudgetTracker.Finance.Services;

public class SubscriberService
{
    private readonly string _subscriberId = "cloud_storage_receipts-sub";
    private readonly string _projectId;
    private readonly ILogger<SubscriberService> _logger;

    public SubscriberService(ILogger<SubscriberService> logger)
    {
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID") ?? throw new ProjectNotFoundException();
        // _subscriberId = appSecrets.BigQuery.DateWiseTransactionSubscriber;
        _logger = logger;
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken) // TODO: Reduce if/else nesting below
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriberId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

        cancellationToken.Register(() =>
        {
            _logger.LogWarning("Cancellation token signaled, stopping SubscriberClient = {Subscriber} in 5 Seconds ...", subscriptionName.SubscriptionId);
            subscriber.StopAsync(TimeSpan.FromSeconds(5));
        });

        await subscriber.StartAsync(async (PubsubMessage message, CancellationToken _) =>
        {
            try
            {
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                _logger.LogInformation("Received message: {Text}", text);
                return SubscriberClient.Reply.Ack;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception in SubscriberService.SubscribeAsync with Message = {ExceptionMessage}", e.InnerException?.Message ?? e.Message);
                return SubscriberClient.Reply.Nack;
            }
        });

        // TODO: This should log after processing a message. But also logging when this subscriber is stopped
        // _logger.LogInformation("Listening for messages on Subscriber {Subscriber}", subscriptionName.SubscriptionId);
    }
}