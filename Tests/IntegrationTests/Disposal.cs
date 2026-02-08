using BudgetTracker.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public sealed class FinanceDbDisposal : IAsyncDisposable
{
    private readonly FinanceTestWebApplicationFactory _factory;

    public FinanceDbDisposal(FinanceTestWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    public async ValueTask DisposeAsync()
    {
        using (IServiceScope scope = _factory.Services.CreateScope())
        {
            WriteDbContext db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            await db.Transactions.ExecuteDeleteAsync();
        }
    }
}