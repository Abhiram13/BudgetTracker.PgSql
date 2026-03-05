using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using IntegrationTests.Data.Transactions;
using IntegrationTests.Definations.Transactions;
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

    [Theory]
    [MemberData(nameof(InsertTransactionsMemberTestData.HappyPathData), MemberType = typeof(InsertTransactionsMemberTestData))]
    public async Task InsertTransaction_201_SuccessResponse(InsertTransactionDto payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", payload);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == "First Transaction #1").ToListAsync();
            
                Assert.NotNull(transactions);
                Assert.NotNull(apiResponse);
                Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.Created, apiResponse.StatusCode);
            }
        }
    }

    [Theory]
    [ClassData(typeof(TransactionsInsertDateValidationTestData))]
    public async Task InsertTransaction_DateValidation_ReturnsExpectedStatus(InsertTransactionDateDef payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                string description = "Date validation Transaction #1";
                InsertTransactionDto insertDto = new InsertTransactionDto
                {
                    ActualAmount = 200,
                    Amount = 200,
                    CategoryId = _testCategory.Id,
                    Description = description,
                    Type = TransactionType.Debit,
                    FromBank = _testBank.Id,
                    ToBank = null,
                    Date = payload.Date,
                };
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == description).ToListAsync();
                
                Assert.NotNull(transactions);
                Assert.NotNull(apiResponse);
                Assert.Equal(payload.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.Equal(payload.ExpectedApiStatusCode, apiResponse.StatusCode);
                Assert.Equal(payload.ShouldDataExists, transactions.Count != 0);
            }
        }
    }

    [Fact]
    public async Task InsertTransaction_FromAndToBankEmpty_BadRequest()
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
                    Description = "From and To bank empty Transaction #1",
                    Type = TransactionType.Debit,
                    FromBank = null,
                    ToBank = null,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                };
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == "From and To bank empty Transaction #1").ToListAsync();
            
                Assert.Empty(transactions);
                Assert.NotNull(apiResponse);
                Assert.True(transactions.Count == 0);
                Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
                Assert.Equal("Invalid Payload exception occured. Please check logs for more details", apiResponse.Message);
            }
        }
    }
}