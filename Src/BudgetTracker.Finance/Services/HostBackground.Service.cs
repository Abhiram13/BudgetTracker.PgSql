namespace BudgetTracker.Finance.Services;

public class FinanceHostBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FinanceHostBackgroundService> _logger;

    public FinanceHostBackgroundService(IServiceScopeFactory scopeFactory, ILogger<FinanceHostBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Subscriber background service starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (IServiceScope scope = _scopeFactory.CreateScope())
                {
                    SubscriberService subscriberService = scope.ServiceProvider.GetRequiredService<SubscriberService>();
                    await subscriberService.SubscribeAsync(stoppingToken); // Keep listening
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in subscriber loop");
                await Task.Delay(5000, stoppingToken); // backoff before retry
            }
        }

        _logger.LogInformation("Subscriber background service stopping.");
    }
}