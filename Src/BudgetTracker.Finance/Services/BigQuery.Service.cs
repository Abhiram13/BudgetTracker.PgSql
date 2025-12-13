using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Finance.Services;

public class BigQueryService
{
    private readonly HttpClient _httpClient;

    public BigQueryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BIGQUERY_SERVER")!);
    }

    public async Task InsertTransactionsByDateAsync(DateTransactionsDto payload)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/query/transactionsByDate", payload);
        string jsonResponse = await response.Content.ReadAsStringAsync();
    }
}