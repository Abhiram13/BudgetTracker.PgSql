using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using IntegrationTests.Finance.Data.Dues;
using IntegrationTests.Finance.Disposals;
using IntegrationTests.Finance.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Finance.Tests.Dues;

[Collection(nameof(DatabaseFixture))]
public class DueTests : IClassFixture<DuesTestsFixture>
{
    private readonly HttpClient _client;
    private readonly DuesTestsFixture _fixture;
    private const string DUE_ROUTE = "/api/dues";

    public DueTests(DuesTestsFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }
    
    [Theory]
    [ClassData(typeof(DuesEntityValidTestData))]
    public async Task Insert_Due_Valid_Success_Async(InsertDueDto payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new DueDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(DUE_ROUTE, payload);
                ApiResponse? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();
                
                Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
                Assert.NotNull(apiResponse);
                Assert.Equal(HttpStatusCode.Created, apiResponse.StatusCode);
                // Assert.NotNull(apiResponse.TraceId); // TODO: Trace ID is null
                // Assert.NotEmpty(apiResponse.TraceId);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
            }
        }
    }
}