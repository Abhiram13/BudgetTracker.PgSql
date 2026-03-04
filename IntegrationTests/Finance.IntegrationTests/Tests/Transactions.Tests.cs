using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Tests;

public class TransactionsTests : IClassFixture<IntegrationTestFixture>
{
    private readonly Category _testCategory;
    private readonly Bank _testBank;
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public TransactionsTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _testCategory = fixture.TestCategory;
        _testBank = fixture.TestBank;
        _fixture = fixture;
    }

    [Fact]
    public async Task InsertTransaction_201_SuccessResponse()
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                InsertTransactionDto insertDto = new InsertTransactionDto
                {
                    ActualAmount = 200,
                    Amount = 200,
                    CategoryId = _testCategory.Id,
                    Description = "First Transaction #1",
                    Type = TransactionType.Debit,
                    FromBank = _testBank.Id,
                    ToBank = null,
                    Date = new DateOnly(2026, 01, 01),
                };
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == "First Transaction #1").ToListAsync();
            
                Assert.NotNull(transactions);
                Assert.NotNull(apiResponse);
                Assert.True(transactions.Count == 1);
                Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.Created, apiResponse.StatusCode);
            }
        }
    }
}