using System.Net.Http.Json;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using IntegrationTests.Setup;

namespace IntegrationTests.Tests;

public class TransactionsTests : IClassFixture<IntegrationTestFixture>
{
    private readonly Category _testCategory;
    private readonly Bank _testBank;
    private readonly HttpClient _client;

    public TransactionsTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _testCategory = fixture.TestCategory;
        _testBank = fixture.TestBank;
    }

    [Fact]
    public async Task InsertTransaction_200_SuccessResponse()
    {
        InsertTransactionDto insertDto = new InsertTransactionDto
        {
            ActualAmount = 200,
            Amount = 200,
            CategoryId = _testCategory.Id,
            Description = "",
            Type = TransactionType.Debit,
            FromBank = _testBank.Id,
            ToBank = null,
            Date = new DateOnly(2026, 01, 01),
        };
        
        HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
        ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
        
        Assert.Equal(200, (int)httpResponse.StatusCode);
    }
}