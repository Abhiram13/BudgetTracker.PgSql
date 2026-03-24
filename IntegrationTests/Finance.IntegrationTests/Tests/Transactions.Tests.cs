using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Models;
using IntegrationTests.Data.Transactions;
using IntegrationTests.Definations.Transactions;
using IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Tests;

public class TransactionsTests : IClassFixture<IntegrationTestFixture>
{
    private readonly Category _testCategory;
    private readonly Bank _testBank;
    private readonly HttpClient _client;
    private readonly HttpClient _unAuthorizedClient;
    private readonly IntegrationTestFixture _fixture;

    public TransactionsTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _unAuthorizedClient = fixture.UnauthorizedClient;
        _testCategory = fixture.TestCategory;
        _testBank = fixture.TestBank;
        _fixture = fixture;
    }

    [Fact]
    public async Task Unauthorised_401_Response_Async()
    {
        string date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        HttpResponseMessage httpResponse = await _unAuthorizedClient.GetAsync($"/api/transactions/date/{date}");
        ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
        
        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.Unauthorized, apiResponse.StatusCode);
        Assert.NotNull(apiResponse.Message);
        Assert.NotEmpty(apiResponse.Message);
    }

    #region Insert Transactions

    [Theory]
    [MemberData(nameof(InsertTransactionsMemberTestData.HappyPathData), MemberType = typeof(InsertTransactionsMemberTestData))]
    public async Task InsertTransaction_201_SuccessResponse(InsertTransactionDto payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", payload);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == "First Transaction #1").ToListAsync();
            
                Assert.NotNull(transactions);
                Assert.NotNull(apiResponse);
                Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.Created, apiResponse.StatusCode);
            }
        }
    }

    [Theory]
    [MemberData(nameof(InsertTransactionsMemberTestData.BadRequestValidationData), MemberType = typeof(InsertTransactionsMemberTestData))]
    public async Task InsertTransaction_Validation_BadRequestResponse(InsertTransactionDto payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", payload);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.ToListAsync();
            
                Assert.Empty(transactions);
                Assert.NotNull(apiResponse);
                Assert.Null(apiResponse.Result);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
            }
        }
    }

    [Theory]
    [ClassData(typeof(TransactionsInsertDateValidationTestData))]
    public async Task InsertTransaction_DateValidation_ReturnsExpectedStatus(InsertTransactionDateDef payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                string description = "Date validation Transaction #1";
                InsertTransactionDto insertDto = new InsertTransactionDto
                {
                    ActualAmount = 200,
                    Amount = 200,
                    CategoryId = _testCategory.Id,
                    Description = description,
                    Type = TransactionType.Debit,
                    FromBank = _testBank.Id,
                    ToBank = null,
                    Date = payload.Date,
                };
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == description).ToListAsync();
                
                Assert.NotNull(transactions);
                Assert.NotNull(apiResponse);
                Assert.Equal(payload.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.Equal(payload.ExpectedApiStatusCode, apiResponse.StatusCode);
                Assert.Equal(payload.ShouldDataExists, transactions.Count != 0);
            }
        }
    }

    [Theory]
    [ClassData(typeof(TransactionsInsertDebitCreditBusinessTestData))]
    public async Task InsertTransaction_DebitCredit_Rules_200_400_Response(InsertTransactionDebitCreditBusinessDataDef payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                string description = "Date validation Transaction #1";
                InsertTransactionDto insertDto = new InsertTransactionDto
                {
                    ActualAmount = 200,
                    Amount = 200,
                    CategoryId = _testCategory.Id,
                    Description = description,
                    Type = payload.TransactionType,
                    FromBank = payload.FromBank,
                    ToBank = payload.ToBank,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                };
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == description).ToListAsync();
                
                Assert.NotNull(transactions);
                Assert.NotNull(apiResponse);
                Assert.Equal(payload.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.Equal(payload.ExpectedApiStatusCode, apiResponse.StatusCode);
            }
        }
    }

    [Theory]
    [ClassData(typeof(TransactionsInsertSecurityEdgeCasesTestData))]
    public async Task InsertTransaction_SecurityEdge_400_Response(InsertTransactionSecurityEdgeCasesDataDef payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                InsertTransactionDto insertDto = new InsertTransactionDto
                {
                    ActualAmount = payload.ActualAmount,
                    Amount = payload.Amount,
                    CategoryId = _testCategory.Id,
                    Description = payload.Description,
                    Type = TransactionType.Debit,
                    FromBank = _testBank.Id,
                    ToBank = null,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                };
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("/api/transactions", insertDto);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                List<Transaction> transactions = await dbContext.Transactions.ToListAsync();
                
                Assert.Empty(transactions);
                Assert.NotNull(apiResponse);
                Assert.Null(apiResponse.Result);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
                Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
            }
        }
    }
    
    #endregion

    #region Transactions By Date

    [Fact]
    public async Task TransactionByDate_DebitTransactions_SuccessResponse()
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                List<Transaction> transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        ActualAmount = 100, 
                        Amount = 100, 
                        CategoryId = 1, 
                        Description = "smome description",
                        Type = TransactionType.Debit,
                        FromBank = _testBank.Id,
                        ToBank = null,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                        UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    },
                    new Transaction
                    {
                        ActualAmount = 100, 
                        Amount = 100, 
                        CategoryId = 1, 
                        Description = "smome description",
                        Type = TransactionType.Debit,
                        FromBank = _testBank.Id,
                        ToBank = null,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                        UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    },
                };

                foreach (Transaction transaction in transactions)
                {
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                }
                
                string date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
                HttpResponseMessage httpResponse = await _client.GetAsync($"/api/transactions/date/{date}");
                ApiResponse<TransactionByDateDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TransactionByDateDto>>();
                
                Assert.NotNull(apiResponse);
                Assert.NotNull(apiResponse.Result);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
                Assert.Equal(200, apiResponse.Result.Debit);
                Assert.Equal((decimal) 0.0, apiResponse.Result.Credit);
                Assert.NotEmpty(apiResponse.Result.TransactionsList);
                Assert.True(apiResponse.Result.TransactionsList.Count == 2);

                foreach (TransactionByDateDto.Transactions result in apiResponse.Result.TransactionsList)
                {
                    Assert.Equal(100, result.Amount);
                    Assert.NotEmpty(result.Description);
                    Assert.Equal(TransactionType.Debit, result.Type);
                }
            }
        }
    }
    
    [Fact]
    public async Task TransactionByDate_CreditTransactions_SuccessResponse()
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                List<Transaction> transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        ActualAmount = 100, 
                        Amount = 100, 
                        CategoryId = 1, 
                        Description = "some description",
                        Type = TransactionType.Credit,
                        FromBank = null,
                        ToBank = _testBank.Id,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                        UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    },
                    new Transaction
                    {
                        ActualAmount = 100, 
                        Amount = 100, 
                        CategoryId = 1, 
                        Description = "some description",
                        Type = TransactionType.Credit,
                        FromBank = null,
                        ToBank = _testBank.Id,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                        UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    },
                };

                foreach (Transaction transaction in transactions)
                {
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                }
                
                string date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
                HttpResponseMessage httpResponse = await _client.GetAsync($"/api/transactions/date/{date}");
                ApiResponse<TransactionByDateDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TransactionByDateDto>>();
                
                Assert.NotNull(apiResponse);
                Assert.NotNull(apiResponse.Result);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
                Assert.Equal(200, apiResponse.Result.Credit);
                Assert.Equal((decimal) 0.0, apiResponse.Result.Debit);
                Assert.NotEmpty(apiResponse.Result.TransactionsList);
                Assert.True(apiResponse.Result.TransactionsList.Count == 2);

                foreach (TransactionByDateDto.Transactions result in apiResponse.Result.TransactionsList)
                {
                    Assert.Equal(100, result.Amount);
                    Assert.NotEmpty(result.Description);
                    Assert.Equal(TransactionType.Credit, result.Type);
                }
            }
        }
    }

    [Theory]
    [ClassData(typeof(TransactionsByDateInvalidOfFutureTestData))]
    public async Task TransactionByDate_DebitTransactions_InvalidOrFutureDate_BadResponse(string date)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.GetAsync($"/api/transactions/date/{date}");
                ApiResponse<TransactionByDateDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TransactionByDateDto>>();
                
                Assert.NotNull(apiResponse);
                Assert.Null(apiResponse.Result);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
                Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
            }
        }
    }

    #endregion
    
    #region Transactions Count by month and year
    
    [Theory]
    [ClassData(typeof(TransactionsByMonthYearTestsData))]
    public async Task TransactionsCount_ByMonthAndYear(TransactionsByMonthYearDataDef data)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                List<Transaction> transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        ActualAmount = 100, 
                        Amount = 100, 
                        CategoryId = 1, 
                        Description = "some description",
                        Type = TransactionType.Debit,
                        FromBank = _testBank.Id,
                        ToBank = null,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                        UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    },
                    new Transaction
                    {
                        ActualAmount = 100, 
                        Amount = 100, 
                        CategoryId = 1, 
                        Description = "some description",
                        Type = TransactionType.Debit,
                        FromBank = _testBank.Id,
                        ToBank = null,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1),
                        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                        UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    },
                };

                foreach (Transaction transaction in transactions)
                {
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                }
                
                string url = "/api/transactions/count?";

                if (data.Month.HasValue && data.Year.HasValue)
                {
                    url += "month=" + data.Month + "&year=" + data.Year;
                }
                else if (data.Month.HasValue)
                {
                    url += "month=" + data.Month;
                }
                else if (data.Year.HasValue)
                {
                    url += "year=" + data.Year;
                }
                
                HttpResponseMessage httpResponse = await _client.GetAsync(url);
                ApiResponse<int>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
                
                Assert.NotNull(apiResponse);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.Equal(data.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.Equal(data.ExpectedApiStatusCode, apiResponse.StatusCode);
                Assert.Equal(data.ShouldDataExists, apiResponse.Result > 0);
            }
        }
    }
    
    #endregion

    [Fact]
    public async Task UpdateTransaction_SuccessResponse_Async()
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                Transaction transaction = new Transaction
                {
                    ActualAmount = 100,
                    Amount = 100,
                    CategoryId = 1,
                    Description = "smome description",
                    Type = TransactionType.Debit,
                    FromBank = _testBank.Id,
                    ToBank = null,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                    UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                };
                
                await dbContext.Transactions.AddAsync(transaction);
                await dbContext.SaveChangesAsync();
                
                UpdateTransactionDto updatePayload = new UpdateTransactionDto
                {
                    ActualAmount = 101,
                    Amount = 101,
                    CategoryId = 1,
                    Description = "updated description",
                    Type = TransactionType.Credit,
                    FromBank = null,
                    ToBank = _testBank.Id,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                };
                
                HttpResponseMessage httpResponse = await _client.PutAsJsonAsync($"/api/transactions/{transaction.Id}", updatePayload);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                Transaction? updatedTransaction = await dbContext.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == transaction.Id);
                
                Assert.NotNull(apiResponse);
                Assert.NotNull(updatedTransaction);
                Assert.NotNull(apiResponse.Message);
                Assert.NotNull(apiResponse.TraceId);
                Assert.NotEmpty(apiResponse.Message);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.Null(apiResponse.Result);
                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
                Assert.Equal(updatePayload.ActualAmount, updatedTransaction.ActualAmount);
                Assert.Equal(updatePayload.Amount, updatedTransaction.Amount);
                Assert.Equal(updatePayload.CategoryId, updatedTransaction.CategoryId);
                Assert.Equal(updatePayload.Description, updatedTransaction.Description);
                Assert.Equal(updatePayload.Type, updatedTransaction.Type);
                Assert.Equal(updatePayload.FromBank, updatedTransaction.FromBank);
                Assert.Equal(updatePayload.ToBank, updatedTransaction.ToBank);
                Assert.Equal(updatePayload.Date, updatedTransaction.Date);
                Assert.NotEqual(updatedTransaction.CreatedAt, updatedTransaction.UpdatedAt);
            }
        }
    }
    
    [Fact]
    public async Task UpdateTransaction_BadRequestResponse_Async()
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new FinanceDbDisposal(dbContext))
            {
                Transaction transaction = new Transaction
                {
                    ActualAmount = 100,
                    Amount = 100,
                    CategoryId = 1,
                    Description = "smome description",
                    Type = TransactionType.Debit,
                    FromBank = _testBank.Id,
                    ToBank = null,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                    UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                };
                
                await dbContext.Transactions.AddAsync(transaction);
                await dbContext.SaveChangesAsync();
                
                UpdateTransactionDto updatePayload = new UpdateTransactionDto
                {
                    ActualAmount = 999999999,
                    Amount = 999999999,
                    CategoryId = 1,
                    Description = "updated description$#@^",
                    Type = TransactionType.Credit,
                    FromBank = _testBank.Id,
                    ToBank = null,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                };
                
                HttpResponseMessage httpResponse = await _client.PutAsJsonAsync($"/api/transactions/{transaction.Id}", updatePayload);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                
                Assert.NotNull(apiResponse);
                Assert.NotNull(apiResponse.Message);
                Assert.NotNull(apiResponse.TraceId);
                Assert.NotEmpty(apiResponse.Message);
                Assert.NotEmpty(apiResponse.TraceId);
                Assert.Null(apiResponse.Result);
                Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
            }
        }
    }
}