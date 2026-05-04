using Abhiram.Secrets.Providers.Exceptions;
using Google.Cloud.PubSub.V1;

namespace BudgetTracker.Finance.BackgroundWorkers;

public class SubscriberBackgroundWorker : BackgroundService
{
    private readonly ILogger<SubscriberBackgroundWorker> _logger;
    private readonly SubscriberClient _subscriberClient;
    private readonly Guid _workerId = Guid.NewGuid();

    public SubscriberBackgroundWorker(ILogger<SubscriberBackgroundWorker> logger, SubscriberClient subscriberClient)
    {
        _logger = logger;
        _subscriberClient = subscriberClient;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Name} with Guid = {Id} is starting...", nameof(SubscriberBackgroundWorker), _workerId);
        
        stoppingToken.Register(() =>
        {
            _logger.LogWarning("Cancellation token signaled, stopping worker with Id = {Id} and SubscriberClient = {Subscriber} in 5 Seconds ...", _workerId, _subscriberClient.SubscriptionName);
            _subscriberClient.StopAsync(TimeSpan.FromSeconds(5));
        });
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SubscribeAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception at {MethodName} in Worker = {WorkerName} with WorkerId = {Id}", nameof(SubscribeAsync), nameof(SubscriberBackgroundWorker), _workerId);
                await Task.Delay(5000, stoppingToken);
            }
        }
        
        _logger.LogInformation("{Name} with Guid = {Id} is stopping...", nameof(SubscriberBackgroundWorker), _workerId);
    }

    private async Task SubscribeAsync(CancellationToken _) // TODO: Reduce if/else nesting below
    {
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
                _logger.LogError(e, "Exception in SubscriberService.SubscribeAsync with WorkerId = {Id} with Message = {ExceptionMessage}", _workerId, e.InnerException?.Message ?? e.Message);
                return SubscriberClient.Reply.Nack;
            }
        });

        // TODO: This should log after processing a message. But also logging when this subscriber is stopped
        // _logger.LogInformation("Listening for messages on Subscriber {Subscriber}", subscriptionName.SubscriptionId);
    }
}