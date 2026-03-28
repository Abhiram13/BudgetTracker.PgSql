using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Finance;
using BudgetTracker.Shared.Models;
using IntegrationTests.Finance.Data.Categories;
using IntegrationTests.Finance.Definations.Categories;
using IntegrationTests.Finance.Disposals;
using IntegrationTests.Finance.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Finance.Tests.Categories;

[Collection(nameof(DatabaseFixture))]
public class CategoryTests
{
    private readonly HttpClient _client;
    private readonly HttpClient _unAuthorisedClient;
    private readonly CategoriesTestsFixture _fixture;
    private const string CATEGORY_ROUTE = "/api/categories";

    public CategoryTests(CategoriesTestsFixture fixture)
    {
        _client = fixture.Client;
        _unAuthorisedClient = fixture.UnAuthorizedClient;
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Unauthorised_401_Response_Async()
    {
        HttpResponseMessage httpResponse = await _unAuthorisedClient.GetAsync(CATEGORY_ROUTE);
        ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
        
        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.Unauthorized, apiResponse.StatusCode);
        Assert.NotNull(apiResponse.Message);
        Assert.NotEmpty(apiResponse.Message);
    }

    [Theory]
    [ClassData(typeof(InsertCategoriesTestData))]
    public async Task Insert_Categories_SuccessFailResponse_Async(InsertCategoryDef testData)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new CategoryDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(CATEGORY_ROUTE, testData.Payload);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                
                Assert.Equal(testData.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.NotNull(apiResponse);
                Assert.Equal(testData.ExpectedApiStatusCode, apiResponse.StatusCode);
                Assert.NotNull(apiResponse.TraceId);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.Null(apiResponse.Result);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
            }
        }
    }
}