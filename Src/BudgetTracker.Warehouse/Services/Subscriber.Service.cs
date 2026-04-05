using System.Text.Json;
using Abhiram.Secrets.Providers.Exceptions;
using Google.Cloud.PubSub.V1;
using BudgetTracker.Shared.Models;
using BudgetTracker.Warehouse.Models;

namespace BudgetTracker.Warehouse.Services;

public class SubscriberService
{
    private readonly string _subscriberId;
    private readonly string _projectId;
    private readonly ILogger<SubscriberService> _logger;
    private readonly BigQueryService _bigQueryService;

    public SubscriberService(BigQueryService service, ILogger<SubscriberService> logger, WarehouseAppSecrets appSecrets)
    {
        _bigQueryService = service;
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID") ?? throw new ProjectNotFoundException();
        _subscriberId = appSecrets.BigQuery.DateWiseTransactionSubscriber;
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
                message.Attributes.TryGetValue("trace-id", out string traceId);
                message.Attributes.TryGetValue("event", out string eventName);

                if (eventName == PubSubFinanceEvents.DATEWISE_TRANSACTIONS_LIST)
                {
                    TransactionCreditDebitByDateDto? resultObject = JsonSerializer.Deserialize<TransactionCreditDebitByDateDto>(text);
                
                    if (resultObject is not null)
                    {
                        try
                        {
                            _logger.LogInformation("Received message at Subscriber = {SubscriptionName} with Trace-Id = {TraceId}", subscriptionName.SubscriptionId, traceId);
                            await _bigQueryService.InsertTransactionByDateAsync(resultObject);
                            return SubscriberClient.Reply.Ack;
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Exception at Inserting data into Big Query. Message = {0} and Trace ID = {1}", e.Message, traceId);
                            return SubscriberClient.Reply.Nack;
                        }                    
                    }
                    
                    _logger.LogWarning("No Data = {Data} was received to the Subscriber = {Subscriber} with Trace-Id = {TraceId}", text, subscriptionName.SubscriptionId, traceId);
                    return SubscriberClient.Reply.Ack;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception in SubscriberService.SubscribeAsync with Message = {ExceptionMessage}", e.InnerException?.Message ?? e.Message);
                return SubscriberClient.Reply.Nack;
            }

            return SubscriberClient.Reply.Ack;
        });

        // TODO: This should log after processing a message. But also logging when this subscriber is stopped
        // _logger.LogInformation("Listening for messages on Subscriber {Subscriber}", subscriptionName.SubscriptionId);
    }
}