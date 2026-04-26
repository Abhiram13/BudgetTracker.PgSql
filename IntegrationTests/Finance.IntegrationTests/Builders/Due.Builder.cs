using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;

namespace IntegrationTests.Finance.Builders;

public sealed class DueBuilder
{
    private readonly WriteDbContext _dbContext;

    public DueBuilder(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Due> CreateDueAsync()
    {
        Due due = Due.Create(
            debtor: "John Doe", 
            creditor: "Sammy", 
            startDate: DateOnly.FromDateTime(DateTime.Today),
            description: "Sample description",
            comments: "Sample comments",
            remarks: "Sample remarks",
            title: "Sample title",
            totalAmount: 100
        );
        
        await _dbContext.Dues.AddAsync(due);
        await _dbContext.SaveChangesAsync();
        
        return due;
    }
}