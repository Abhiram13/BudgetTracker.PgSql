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
        Bank bank = Bank.Create("Test bank");
        
        await _dbContext.Banks.AddAsync(bank);
        await _dbContext.SaveChangesAsync();
        
        return bank;
    }
}