using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Exceptions;
using BudgetTracker.Shared.Models;
using IntegrationTests.Finance.Data.Transactions;
using IntegrationTests.Finance.Definations.Transactions;
using IntegrationTests.Finance.Fixtures;
using IntegrationTests.Finance.Disposals;
using Xunit.Abstractions;

namespace IntegrationTests.Finance.Tests.Transactions;

[Collection(nameof(DatabaseFixture))]
public class TransactionsTests : IClassFixture<TransactionsIntegrationTestFixture>
{
    private readonly Category _testCategory;
    private readonly Bank _testBank;
    private readonly HttpClient _client;
    private readonly HttpClient _unAuthorizedClient;
    private readonly TransactionsIntegrationTestFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;
    private const string TRANSACTIONS_ROUTE = "/api/transactions";

    public TransactionsTests(TransactionsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _client = fixture.Client;
        _unAuthorizedClient = fixture.UnAuthorizedClient;
        _testCategory = fixture.TestCategory;
        _testBank = fixture.TestBank;
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    // TODO: Fix the response format
    // [Fact]
    // public async Task Unauthorised_401_Response_Async()
    // {
    //     string date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
    //     HttpResponseMessage httpResponse = await _unAuthorizedClient.GetAsync($"{TRANSACTIONS_ROUTE}/date/{date}");
    //     ApiResponse? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();
    //     
    //     Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
    //     Assert.NotNull(apiResponse);
    //     Assert.Equal(HttpStatusCode.Unauthorized, apiResponse.StatusCode);
    //     Assert.NotNull(apiResponse.Message);
    //     Assert.NotEmpty(apiResponse.Message);
    // }
    
    #region Transaction Entity

    [Theory]
    [ClassData(typeof(TransactionsEntityValidTestData))]
    public async Task Transaction_Entity_Valid_Success_Async(Transaction transaction)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbcontext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            
            await using (new TransactionDisposal(dbcontext))
            {
                await dbcontext.Transactions.AddAsync(transaction);
                await dbcontext.SaveChangesAsync();
                
                Transaction? data = await dbcontext.Transactions.Where(t => t.Description == transaction.Description).FirstOrDefaultAsync();
                
                Assert.NotNull(data);
                Assert.Equal(transaction.Description, data.Description);
            }
        }
    }

    [Theory]
    [ClassData(typeof(TransactionsEntityInValidTestData))]
    public async Task Transaction_Entity_InValid_ThrowsException_Async(InsertTransactionInvalidEntityThrowsExceptionDto data)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbcontext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            await using (new TransactionDisposal(dbcontext))
            {
                Exception exception = await Record.ExceptionAsync(async () =>
                {
                    Transaction transaction = Transaction.Create(
                        actualAmount: data.Payload.Amount,
                        description: data.Payload.Description,
                        amount: data.Payload.Amount,
                        date: data.Payload.Date,
                        categoryId: data.Payload.CategoryId,
                        fromBank: data.Payload.FromBank,
                        toBank: data.Payload.ToBank,
                        type: data.Payload.Type
                    );
                    await dbcontext.Transactions.AddAsync(transaction);
                    await dbcontext.SaveChangesAsync();
                });
                
                Assert.NotNull(exception);
                Assert.IsType(data.ExpectedExceptionType, exception);
            }
        }
    }

    #endregion

    #region Insert Transactions

    [Theory]
    [MemberData(nameof(InsertTransactionsMemberTestData.HappyPathData), MemberType = typeof(InsertTransactionsMemberTestData))]
    public async Task InsertTransaction_201_SuccessResponse(InsertTransactionDto payload)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new TransactionDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(TRANSACTIONS_ROUTE, payload);
                ApiResponse<InsertTransactionResponseDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<InsertTransactionResponseDto>>();
                List<Transaction> transactions = await dbContext.Transactions.Where(t => t.Description == payload.Description).ToListAsync();
            
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

            await using (new TransactionDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(TRANSACTIONS_ROUTE, payload);
                ApiResponse<InsertTransactionResponseDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<InsertTransactionResponseDto>>();
                List<Transaction> transactions = await dbContext.Transactions.ToListAsync();
            
                Assert.Empty(transactions);
                Assert.NotNull(apiResponse);
                Assert.Null(apiResponse.Result);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
                // Assert.NotEmpty(apiResponse.TraceId); // TODO: Trace ID is null
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

            await using (new TransactionDisposal(dbContext))
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
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(TRANSACTIONS_ROUTE, insertDto);
                ApiResponse<InsertTransactionResponseDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<InsertTransactionResponseDto>>();
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

            await using (new TransactionDisposal(dbContext))
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
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(TRANSACTIONS_ROUTE, insertDto);
                ApiResponse<InsertTransactionResponseDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<InsertTransactionResponseDto>>();
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

            await using (new TransactionDisposal(dbContext))
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
        
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(TRANSACTIONS_ROUTE, insertDto);
                ApiResponse<InsertTransactionResponseDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<InsertTransactionResponseDto>>();
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
    
    [Theory]
    [ClassData(typeof(TransactionsInsertDueMetaSuccessTestData))]
    public async Task InsertTransaction_DueInsert_TransactionsMeta_SuccessResponse_Async(InsertTransactionDueIdMetaDataDef data)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new TransactionDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(TRANSACTIONS_ROUTE, data.Payload);
                ApiResponse<InsertTransactionResponseDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<InsertTransactionResponseDto>>();
                TransactionsMeta? meta = await dbContext.TransactionsMeta.Where(m => m.TransactionId == apiResponse!.Result.TransactionId).FirstOrDefaultAsync();
                
                Assert.NotNull(apiResponse);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
                Assert.NotNull(apiResponse.Result);
                Assert.Equal(data.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.Equal(data.ExpectedApiStatusCode, apiResponse.StatusCode);
                Assert.Equal(data.ExpectedMetaData, meta is not null);

                if (data.ExpectedMetaData)
                {
                    Assert.Equal(meta!.DueId, data.Payload.DueId);
                }
            }
        }
    }
    
    [Theory]
    [ClassData(typeof(TransactionsInsertDueMetaFailureTestData))]
    public async Task InsertTransaction_DueInsert_TransactionsMeta_FailureResponse_Async(InsertTransactionDueIdMetaDataDef data)
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new TransactionDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(TRANSACTIONS_ROUTE, data.Payload);
                ApiResponse? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();
                int? metaDataCount = await dbContext.TransactionsMeta.CountAsync();
                
                Assert.NotNull(apiResponse);
                Assert.NotNull(apiResponse.Message);
                Assert.NotEmpty(apiResponse.Message);
                Assert.Equal(0, metaDataCount);
                Assert.Equal(data.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.Equal(data.ExpectedApiStatusCode, apiResponse.StatusCode);
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

            await using (new TransactionDisposal(dbContext))
            {
                List<Transaction> transactions = new List<Transaction>
                {
                    Transaction.Create(
                        actualAmount: 100, 
                        amount: 100, 
                        categoryId: 1, 
                        description: "smome description",
                        type: TransactionType.Debit,
                        fromBank: _testBank.Id,
                        toBank: null, 
                        date: DateOnly.FromDateTime(DateTime.UtcNow)
                    ),
                    Transaction.Create(
                        actualAmount: 100, 
                        amount: 100, 
                        categoryId: 1, 
                        description: "smome description",
                        type: TransactionType.Debit,
                        fromBank: _testBank.Id,
                        toBank: null, 
                        date: DateOnly.FromDateTime(DateTime.UtcNow)
                    ),
                };

                foreach (Transaction transaction in transactions)
                {
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                }
                
                string date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
                HttpResponseMessage httpResponse = await _client.GetAsync($"{TRANSACTIONS_ROUTE}/date/{date}");
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

            await using (new TransactionDisposal(dbContext))
            {
                List<Transaction> transactions = new List<Transaction>
                {
                    Transaction.Create(
                        actualAmount: 100, 
                        amount: 100, 
                        categoryId: 1, 
                        description: "smome description",
                        type: TransactionType.Credit,
                        fromBank: null,
                        toBank: _testBank.Id, 
                        date: DateOnly.FromDateTime(DateTime.UtcNow)
                    ),
                    Transaction.Create(
                        actualAmount: 100, 
                        amount: 100, 
                        categoryId: 1, 
                        description: "smome description",
                        type: TransactionType.Credit,
                        fromBank: null,
                        toBank: _testBank.Id, 
                        date: DateOnly.FromDateTime(DateTime.UtcNow)
                    ),
                };

                foreach (Transaction transaction in transactions)
                {
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                }
                
                string date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
                HttpResponseMessage httpResponse = await _client.GetAsync($"{TRANSACTIONS_ROUTE}/date/{date}");
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

            await using (new TransactionDisposal(dbContext))
            {
                HttpResponseMessage httpResponse = await _client.GetAsync($"{TRANSACTIONS_ROUTE}/date/{date}");
                ApiResponse<TransactionByDateDto>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TransactionByDateDto>>();
                
                Assert.NotNull(apiResponse);
                Assert.Null(apiResponse.Result);
                // Assert.NotEmpty(apiResponse.TraceId); // TODO: Getting empty Trace ID
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

            await using (new TransactionDisposal(dbContext))
            {
                List<Transaction> transactions = new List<Transaction>
                {
                    Transaction.Create(
                        actualAmount: 100, 
                        amount: 100, 
                        categoryId: 1, 
                        description: "smome description",
                        type: TransactionType.Debit,
                        fromBank: _testBank.Id,
                        toBank: null, 
                        date: DateOnly.FromDateTime(DateTime.UtcNow)
                    ),
                    Transaction.Create(
                        actualAmount: 100, 
                        amount: 100, 
                        categoryId: 1, 
                        description: "smome description",
                        type: TransactionType.Debit,
                        fromBank: _testBank.Id,
                        toBank: null, 
                        date: DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1)
                    ),
                };

                foreach (Transaction transaction in transactions)
                {
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                }
                
                string url = $"{TRANSACTIONS_ROUTE}/count?";

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
                Assert.NotNull(apiResponse.Result);
                // Assert.NotEmpty(apiResponse.TraceId); // TODO: Empty Trace ID
                Assert.Equal(data.ExpectedHttpStatusCode, httpResponse.StatusCode);
                Assert.Equal(data.ExpectedApiStatusCode, apiResponse.StatusCode);
                Assert.Equal(data.ShouldDataExists, apiResponse.Result > 0);
            }
        }
    }
    
    #endregion

    #region Update transactions
    
    [Fact]
    public async Task UpdateTransaction_SuccessResponse_Async()
    {
        using (IServiceScope scope = _fixture.Factory.CreateScope())
        {
            WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            await using (new TransactionDisposal(dbContext))
            {
                Transaction transaction = Transaction.Create(
                    actualAmount: 100,
                    amount: 100,
                    categoryId: 1,
                    description: "smome description",
                    type: TransactionType.Debit,
                    fromBank: _testBank.Id,
                    toBank: null,
                    date: DateOnly.FromDateTime(DateTime.UtcNow)
                );
                
                transaction.SetTimeStamps(createdAt: DateTimeOffset.UtcNow.AddDays(-1), updatedAt: DateTimeOffset.UtcNow.AddDays(-1));
                
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
                
                HttpResponseMessage httpResponse = await _client.PutAsJsonAsync($"{TRANSACTIONS_ROUTE}/{transaction.Id}", updatePayload);
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

            await using (new TransactionDisposal(dbContext))
            {
                Transaction transaction = Transaction.Create(
                    actualAmount: 100,
                    amount: 100,
                    categoryId: 1,
                    description: "smome description",
                    type: TransactionType.Debit,
                    fromBank: _testBank.Id,
                    toBank: null,
                    date: DateOnly.FromDateTime(DateTime.UtcNow)
                );
                
                transaction.SetTimeStamps(createdAt: DateTimeOffset.UtcNow.AddDays(-1), updatedAt: DateTimeOffset.UtcNow.AddDays(-1));
                
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
                
                HttpResponseMessage httpResponse = await _client.PutAsJsonAsync($"{TRANSACTIONS_ROUTE}/{transaction.Id}", updatePayload);
                ApiResponse<string>? apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
                
                Assert.NotNull(apiResponse);
                Assert.NotNull(apiResponse.Message);
                // Assert.NotNull(apiResponse.TraceId); // TODO: Trace ID is null
                Assert.NotEmpty(apiResponse.Message);
                // Assert.NotEmpty(apiResponse.TraceId); // TODO: Trace ID is null
                Assert.Null(apiResponse.Result);
                Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
            }
        }
    }
    
    #endregion
}