using System.Text.Json;
using Google.Cloud.PubSub.V1;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Warehouse.Services;

public class SubscriberService
{
    private readonly string _subscriberId;
    private readonly string _projectId;
    private readonly ILogger<SubscriberService> _logger;
    private readonly BigQueryService _bigQueryService;

    public SubscriberService(BigQueryService service, ILogger<SubscriberService> logger)
    {
        _bigQueryService = service;
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID") ?? "";
        _subscriberId = Environment.GetEnvironmentVariable("PUBSUB_DATEWISE_TRANSACTIONS_SUBSCRIBER")!;
        _logger = logger;
    }

    public async Task SubscribeAsync()
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriberId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

        await subscriber.StartAsync(async (PubsubMessage message, CancellationToken _) =>
        {
            string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
            string traceId = message.Attributes["trace-id"];

            if (message.Attributes["event"] == PubSubFinanceEvents.DATEWISE_TRANSACTIONS_LIST)
            {
                TransactionsListByMonthDto? resultObject = JsonSerializer.Deserialize<TransactionsListByMonthDto>(text);

                if (resultObject is not null)
                {
                    try
                    {
                        _logger.LogInformation($"Received message at {subscriber.SubscriptionName} subscriber with Trace ID: {traceId}");
                        await _bigQueryService.InsertTransactionByDateAsync(resultObject);
                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Exception at Inserting data into Big Query. Message = {0} and Trace ID = {1}", e.Message, traceId);
                    }                    
                }
                else
                {
                    _logger.LogWarning($"No Data {text} was received to the Subscriber {subscriber} with Trace ID {traceId}");
                    return SubscriberClient.Reply.Nack;
                }
            }

            return SubscriberClient.Reply.Ack;
        });

        _logger.LogInformation($"Listening for messages on {subscriptionName}");
    }
}