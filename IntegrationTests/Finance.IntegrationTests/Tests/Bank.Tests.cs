using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using IntegrationTests.Finance.Data.Banks;
using IntegrationTests.Finance.Disposals;
using IntegrationTests.Finance.Fixtures;
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
    
    [Theory]
    [ClassData(typeof(BankEntityValidTestData))]
    public async Task Insert_Bank_Entity_Valid_Success_Async(string bankName)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbcontext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            
            await using (new BankDisposal(dbcontext))
            {
                Bank bank = Bank.Create(bankName);
                await dbcontext.Banks.AddAsync(bank);
                await dbcontext.SaveChangesAsync();

                Bank? data = await dbcontext.Banks.Where(c => c.Name == bankName).FirstOrDefaultAsync();
                
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
            WriteDbContext dbcontext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            
            await using (new BankDisposal(dbcontext))
            {
                Bank bank = Bank.Create(bankName);
                await Assert.ThrowsAsync<DbUpdateException>(async () =>
                {
                    await dbcontext.Banks.AddAsync(bank);
                    await dbcontext.SaveChangesAsync();
                });
                
                Bank? data = await dbcontext.Banks.Where(c => c.Name == bankName).FirstOrDefaultAsync();
                Assert.Null(data);
            }
        }
    }
}