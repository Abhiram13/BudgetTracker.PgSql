using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;

namespace IntegrationTests.Finance.Builders;

public sealed class BankBuilder
{
    private readonly WriteDbContext _dbContext;

    public BankBuilder(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Bank> CreateBankAsync()
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        Bank bank = new Bank
        {
            Name = "Test bank",
            CreatedAt = date,
            UpdatedAt = date,
        };
        
        await _dbContext.Banks.AddAsync(bank);
        await _dbContext.SaveChangesAsync();
        
        return bank;
    }
}