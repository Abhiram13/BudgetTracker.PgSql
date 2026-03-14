using System.Net.Http.Json;
using BudgetTracker.Shared.Models;
using Google.Cloud.BigQuery.V2;
using Warehouse.IntegrationTests.Services;
using Warehouse.IntegrationTests.Setup;

namespace Warehouse.IntegrationTests;

public class BigQueryTests : IClassFixture<WarehouseTestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly WareHouseService _service;

    public BigQueryTests(WarehouseTestWebApplicationFactory factory, WareHouseService service)
    {
        _client = factory.CreateClient();
        _service = service;
    }
    
    [Fact]
    public async Task TestAsync()
    {
        TransactionsListByMonthDto payload = new TransactionsListByMonthDto
        {
            Count = 10,
            Credit = 100,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Debit = 1000
        };
        
        await _service.InsertTransactionsByMonthAsync(payload);
        
        HttpResponseMessage response = await _client.GetAsync("/api/query/transactionsByMonth");
        ApiResponse<List<TransactionsListByMonthDto>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<TransactionsListByMonthDto>>>();
        response.EnsureSuccessStatusCode();
        
        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Result);
        Assert.True(apiResponse.Result.Count > 0);
        Assert.Equal(100, apiResponse.Result[0].Credit);
    }
}