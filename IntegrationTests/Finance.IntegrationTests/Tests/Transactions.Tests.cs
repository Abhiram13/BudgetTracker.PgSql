using System.Net.Http.Json;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;

namespace IntegrationTests.Tests;

public class TransactionsTests : BaseIntegrationTests
{
    public TransactionsTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task InsertTransaction_200_SuccessResponse()
    {
        InsertTransactionDto insertDto = new InsertTransactionDto
        {
            ActualAmount = 200,
            Amount = 200,
            CategoryId = 1,
            Description = "",
            Type = TransactionType.Debit,
            FromBank = 1,
            ToBank = 1,
            Date = new DateOnly(2026, 01, 01),
        };
        
        HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
        ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
        
        Assert.Equal(200, (int)httpResponse.StatusCode);
    }
}