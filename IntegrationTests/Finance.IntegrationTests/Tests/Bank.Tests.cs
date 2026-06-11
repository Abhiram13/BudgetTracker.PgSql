using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Shared.Exceptions;
using IntegrationTests.Finance.Data.Banks;
using IntegrationTests.Finance.Disposals;
using IntegrationTests.Finance.Fixtures;
using IntegrationTests.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Finance.Tests.Banks;

[Collection(nameof(DatabaseFixture))]
public class BankTests : IClassFixture<BanksTestsFixture>
{
    private readonly HttpClient _client;
    private readonly HttpClient _unAuthorizedClient;
    private readonly BanksTestsFixture _fixture;
    private const string BANK_ROUTE = "/api/banks";

    public BankTests(BanksTestsFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _unAuthorizedClient = fixture.UnAuthorizedClient;
    }
    
    #region Bank Entity Tests
    
    [Theory]
    [ClassData(typeof(BankEntityValidTestData))]
    public async Task Insert_Bank_Entity_Valid_Success_Async(string bankName)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            
            await using (new BankDisposal(dbContext))
            {
                // Insert bank in DB using DBContext
                Bank bank = Bank.Create(bankName);
                await dbContext.Banks.AddAsync(bank);
                await dbContext.SaveChangesAsync();

                // Fetch the inserted Bank using BankName
                Bank data = await dbContext.Banks.Where(c => c.Name == bankName).FirstAsync();
                
                // Verify
                Assert.NotNull(data);
                Assert.Equal(bankName, data.Name);
            }
        }
    }
    
    [Theory]
    [ClassData(typeof(BankEntityInValidTestData))]
    public async Task Insert_Bank_Entity_InValid_Fail_Async(string bankName)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            
            await using (new BankDisposal(dbContext))
            {
                // The below insertion throws 'InvalidPayloadException'. Catching it with Record.ExceptionAsync
                Exception exception = await Record.ExceptionAsync(async () =>
                {
                    // Insert Bank in DB using DBContext
                    Bank bank = Bank.Create(bankName);
                    await dbContext.Banks.AddAsync(bank);
                    await dbContext.SaveChangesAsync();
                });
                
                // Fetch the inserted Bank
                Bank? data = await dbContext.Banks.Where(c => c.Name == bankName).FirstOrDefaultAsync();
                
                // Verify
                Assert.Null(data);
                Assert.NotNull(exception);
                Assert.IsType<InvalidPayloadException>(exception);
            }
        }
    }
    
    #endregion
}