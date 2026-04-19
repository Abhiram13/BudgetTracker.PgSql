using BudgetTracker.Finance.Configurations;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Finance.BackgroundWorkers;

public class OutboxProcessordWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessordWorker> _logger;
    private readonly PublisherService _publisherService;
    private readonly TimeSpan _period;
    private readonly OutboxConfig _outboxConfig;

    public OutboxProcessordWorker(IServiceProvider serviceProvider, ILogger<OutboxProcessordWorker> logger, PublisherService publisherService, AppSecrets appSecrets)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _publisherService = publisherService;
        _outboxConfig = appSecrets.OutboxConfig;
        _period = TimeSpan.FromSeconds(_outboxConfig.Period);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Name} started and will run in background for every {Time} seconds.", nameof(OutboxProcessordWorker), _period.TotalSeconds);
        
        using (PeriodicTimer timer = new PeriodicTimer(_period))
        {
            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken _) // TODO: See how and where to use this CancellationToken
    {
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            OutboxService outboxService = scope.ServiceProvider.GetRequiredService<OutboxService>();
            List<OutboxUnProcessedDto> outboxes = await outboxService.GetProcessingMessagesAsync(maxLimit: 3);

            foreach (OutboxUnProcessedDto outbox in outboxes) // TODO: Need to check more test cases
            {
                try
                {
                    await _publisherService.PublishMessageAsync(
                        requestMessage: outbox.Payload.RootElement.ToString(), 
                        eventType: SharedConstants.PubSubFinanceEvents.DATEWISE_TRANSACTIONS_LIST, 
                        traceId: null
                    ); // TODO: Use publish Id?
                    await outboxService.UpdateSuccessAsync(outbox.Id);
                    _logger.LogInformation("Successfully processed outbox message with Id = {OutboxId}.", outbox.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error processing outbox message with Id = {OutboxId}", outbox.Id);
                    await outboxService.UpdateCountAndErrorAsync(outbox.Id, e.Message);
                }
            }
        }
    }
}