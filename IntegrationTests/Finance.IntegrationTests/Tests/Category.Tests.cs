using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Shared.Exceptions;
using BudgetTracker.Shared.Models;
using IntegrationTests.Finance.Data.Categories;
using IntegrationTests.Finance.Definations.Categories;
using IntegrationTests.Finance.Disposals;
using IntegrationTests.Finance.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Finance.Tests.Categories;

[Collection(nameof(DatabaseFixture))]
public class CategoryTests : IClassFixture<CategoriesTestsFixture>
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

    [Theory]
    [ClassData(typeof(CategoryEntityValidTestData))]
    public async Task Insert_Category_Entity_Valid_Success_Async(string categoryName)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbcontext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            
            await using (new CategoryDisposal(dbcontext))
            {
                Category category = Category.Create(categoryName);
                await dbcontext.Categories.AddAsync(category);
                await dbcontext.SaveChangesAsync();
                
                Category? data = await dbcontext.Categories.Where(c => c.Name == categoryName).FirstOrDefaultAsync();
                
                Assert.NotNull(data);
                Assert.Equal(categoryName, data.Name);
            }
        }
    }
    
    [Theory]
    [ClassData(typeof(CategoryEntityInValidTestData))]
    public async Task Insert_Category_Entity_InValid_ThrowsException_Async(string categoryName)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbcontext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            
            await using (new CategoryDisposal(dbcontext))
            {
                await Assert.ThrowsAsync<InvalidPayloadException>(async () =>
                {
                    Category category = Category.Create(categoryName);
                    await dbcontext.Categories.AddAsync(category);
                    await dbcontext.SaveChangesAsync();
                });
                
                Category? data = await dbcontext.Categories.Where(c => c.Name == categoryName).FirstOrDefaultAsync();
                Assert.Null(data);
            }
        }
    }
    
    // TODO: Fix response format
    // [Fact]
    // public async Task Unauthorised_401_Response_Async()
    // {
    //     HttpResponseMessage httpResponse = await _unAuthorisedClient.GetAsync(CATEGORY_ROUTE);
    //     ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
    //     
    //     Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
    //     Assert.NotNull(apiResponse);
    //     Assert.Equal(HttpStatusCode.Unauthorized, apiResponse.StatusCode);
    //     Assert.NotNull(apiResponse.Message);
    //     Assert.NotEmpty(apiResponse.Message);
    // }

    [Theory]
    [ClassData(typeof(InsertCategoriesTestData))]
    public async Task Insert_Categories_SuccessFailResponse_Async(InsertCategoryDef testData)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new CategoryDisposal(dbContext))
            {
                if (testData.PreSeedData) // Duplicate test. Preseed data
                {
                    await dbContext.Categories.AddAsync(Category.Create(testData.Payload.Name));
                    await dbContext.SaveChangesAsync();
                }
                
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(CATEGORY_ROUTE, testData.Payload);
                ApiResponse? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();
                bool isDataExists = await dbContext.Categories.AnyAsync(c => c.Name == testData.Payload.Name);
                
                Assert.Equal(testData.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.NotNull(apiResponse);
                Assert.Equal(testData.ExpectedApiStatusCode, apiResponse.StatusCode);
                // Assert.NotNull(apiResponse.TraceId); // TODO: Trace ID is null
                // Assert.NotEmpty(apiResponse.TraceId);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);

                if (!testData.PreSeedData) // during preseeding, obviously manual entry data exists in DB. So checking this case only when not preseeding.
                {
                    Assert.Equal(testData.ShouldDataExist, isDataExists);
                }
            }
        }
    }
    
    // public async Task Get_Categories_List_Async()
    // {
    //     HttpResponseMessage httpResponse = await _client.GetAsync(CATEGORY_ROUTE);
    // }
}