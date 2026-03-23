using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Shared.Models;
using Google.Cloud.BigQuery.V2;
using Warehouse.IntegrationTests.Services;
using Warehouse.IntegrationTests.Setup;

namespace Warehouse.IntegrationTests;

public class BigQueryTests : IClassFixture<WarehouseIntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly HttpClient _unAuthorizedClient;
    private readonly WareHouseService _service;

    public BigQueryTests(WarehouseIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _service = fixture.WarehouseService!;
        _unAuthorizedClient = fixture.UnAuthorizedClient;
    }

    [Fact]
    public async Task Should_Throw_Unauthorised_Response_Async()
    {
        HttpResponseMessage httpResponse = await _unAuthorizedClient.GetAsync("/api/query/transactionsByMonth");
        ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
        
        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.Unauthorized, apiResponse.StatusCode);
        Assert.NotNull(apiResponse.TraceId);
        Assert.NotEmpty(apiResponse.TraceId);
        Assert.NotNull(apiResponse.Message);
        Assert.NotEmpty(apiResponse.Message);
    }
    
    [Fact]
    public async Task Fetch_CurrentMonth_transactions_SucessResponse_Async()
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
        Assert.Equal(1000, apiResponse.Result[0].Debit);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Now), apiResponse.Result[0].Date);
        Assert.Equal(10, apiResponse.Result[0].Count);
    }
}