using Abhiram.Secrets.Providers.Exceptions;
using Google.Cloud.PubSub.V1;

namespace BudgetTracker.Finance.BackgroundWorkers;

public class SubscriberBackgroundWorker : BackgroundService
{
    private readonly ILogger<SubscriberBackgroundWorker> _logger;
    private readonly SubscriberClient _subscriberClient;

    public SubscriberBackgroundWorker(ILogger<SubscriberBackgroundWorker> logger, SubscriberClient subscriberClient)
    {
        _logger = logger;
        _subscriberClient = subscriberClient;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(SubscriberBackgroundWorker)} is starting...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SubscribeAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception in {MethodName}", nameof(SubscribeAsync));
                await Task.Delay(5000, stoppingToken);
            }
        }
        
        _logger.LogInformation($"{nameof(SubscriberBackgroundWorker)} is stopping...");
    }

    private async Task SubscribeAsync(CancellationToken cancellationToken) // TODO: Reduce if/else nesting below
    {
        cancellationToken.Register(() =>
        {
            _logger.LogWarning("Cancellation token signaled, stopping SubscriberClient = {Subscriber} in 5 Seconds ...", _subscriberClient.SubscriptionName);
            _subscriberClient.StopAsync(TimeSpan.FromSeconds(5));
        });

        await _subscriberClient.StartAsync(async (PubsubMessage message, CancellationToken _) =>
        {
            try
            {
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                _logger.LogInformation("At Subscription = {SubscriptionName} received the message: {Text}", _subscriberClient.SubscriptionName, text);
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