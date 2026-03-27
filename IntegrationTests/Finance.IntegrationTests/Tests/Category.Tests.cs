using System.Net.Http.Json;
using BudgetTracker.Finance;
using BudgetTracker.Shared.Models;
using IntegrationTests.Finance.Data.Categories;
using IntegrationTests.Finance.Definations.Categories;
using IntegrationTests.Finance.Disposals;
using IntegrationTests.Finance.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Finance.Tests.Categories;

public class CategoryTests : IClassFixture<CategoriesTestsFixture>
{
    private readonly HttpClient _client;
    private readonly CategoriesTestsFixture _fixture;

    public CategoryTests(CategoriesTestsFixture fixture)
    {
        _client = fixture.Client;
        _fixture = fixture;
    }

    [Theory]
    [ClassData(typeof(InsertCategoriesTestData))]
    public async Task Insert_Categories_SuccessFailResponse_Async(InsertCategoryDef testData)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new CategorysDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/categories", testData.Payload);
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