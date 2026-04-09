namespace BudgetTracker.Finance.HttpClients;

public class CloudStorageHttpClient : HttpClient
{
    private readonly HttpClient _httpClient;

    public CloudStorageHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetSignedUrlAsync()
    {
        return await _httpClient.GetStringAsync("api/storage");
    }
}