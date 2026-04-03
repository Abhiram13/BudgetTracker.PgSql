using Abhiram.Secrets.Providers.Exceptions;
using Google.Protobuf;
using Google.Cloud.PubSub.V1;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Services;

/// <summary>
/// <exception cref="EnvironmentVariableNotFoundException">
/// Thrown when <c>GOOGLE_CLOUD_PROJECT_ID</c> is not found
/// </exception>
/// </summary>
public class PublisherService
{
    private readonly string _topicName;
    private readonly ILogger<PublisherService> _logger;
    private readonly string _projectId;

    public PublisherService(ILogger<PublisherService> logger, AppSecrets appSecrets)
    {
        _topicName = appSecrets.PubSub.Topic;
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID")  // TODO: Get project id through AppSecrets
                     ?? throw new EnvironmentVariableNotFoundException("Google cloud project ID EnvironmentVariable not found");
        _logger = logger;
    }

    /// <summary>
    /// Publishes a message to a Google Cloud Pub/Sub topic.
    /// </summary>
    /// <param name="requestMessage">The string content of the message payload.</param>
    /// <param name="traceId">A unique identifier for tracing the message across services. This will be added as an attribute.</param>
    /// <param name="eventType">The type of the event being published (e.g., "file_uploaded"). This will be added as an attribute.</param>
    /// <returns>A task that represents the asynchronous publish operation. The task's result is the unique ID of the published message.</returns>
    public async Task<string> PublishMessageAsync(string requestMessage, string? traceId, string eventType)
    {
        // Create a TopicName object for the request topic.
        TopicName requestTopicName = TopicName.FromProjectTopic(_projectId, _topicName);

        // Create a PublisherClient to publish messages to the request topic.
        PublisherClient requestPublisher = await PublisherClient.CreateAsync(requestTopicName);

        // Create a PubsubMessage object with the request message data.
        PubsubMessage message = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(requestMessage),
            Attributes = {
                {"event", eventType},
                {"trace-id", Guid.NewGuid().ToString()} // TODO: Try to pass request trace-id here. For now, temporarily new GUID will be passed
            }
        };

        // Publish the request message.
        string publishId = await requestPublisher.PublishAsync(message);
        _logger.LogInformation("Message successfully published with ID = {PublishId} to Topic = {TopicName} with Trace-Id = {TraceId}", publishId, _topicName, traceId); // TODO: Get Trace Id here
        
        return publishId;
    }
}