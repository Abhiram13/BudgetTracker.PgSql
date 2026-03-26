using System.Net;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using IntegrationTests.Finance.Definations.Transactions;

namespace IntegrationTests.Finance.Data.Transactions;

public abstract class TheoryTestData<Def> : TheoryData<Def> where Def : class
{
    protected readonly DateOnly _currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
}

#region Insert Transactions

public class TransactionsInsertDateValidationTestData : TheoryTestData<InsertTransactionDateDef>
{
    public TransactionsInsertDateValidationTestData()
    {
        // Past date validation
        Add(new InsertTransactionDateDef
        {
            Date = _currentDate.AddDays(-10),
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ShouldDataExists = true,
        });
        
        // Current date validation
        Add(new InsertTransactionDateDef
        {
            Date = _currentDate,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ShouldDataExists = true,
        });
        
        // Future date validation
        Add(new InsertTransactionDateDef
        {
            Date = _currentDate.AddDays(10),
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ShouldDataExists = false,
        });
    }
}

public static class InsertTransactionsMemberTestData
{
    private static readonly DateOnly _currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
    private static readonly DateOnly _pastDate = _currentDate.AddDays(-10);
    private static readonly DateOnly _futureDate = _currentDate.AddDays(10);
    
    public static IEnumerable<object[]> HappyPathData()
    {
        // Debit, category id, from bank, current date
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // Debit, category id, to bank, current date
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // Debit, category id, from bank, past date
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _pastDate,
            }
        };
        
        // Debit, no actual amount, category id, from bank, past date
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 200,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _pastDate,
            }
        };
        
        // Credit, category id, from bank, current date
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Credit Transaction #1",
                Type = TransactionType.Credit,
                FromBank = null,
                ToBank = 1,
                Date = _currentDate,
            }
        };
        
        // Credit, category id, to bank, current date
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Credit Transaction #1",
                Type = TransactionType.Credit,
                FromBank = null,
                ToBank = 1,
                Date = _currentDate,
            }
        };
        
        // Credit, category id, from bank, past date
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Credit Transaction #1",
                Type = TransactionType.Credit,
                FromBank = null,
                ToBank = 1,
                Date = _pastDate,
            }
        };
    }

    public static IEnumerable<object[]> BadRequestValidationData()
    {
        // amount is 0, debit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 0,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // amount is negative, debit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = -10,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // actual amount is negative, debit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = -10,
                Amount = 100,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // From & To banks are null, debit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = null,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // Description empty, debit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                CategoryId = 1,
                Description = "",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // Category id is invalid, debit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                CategoryId = 0,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
        
        // invalid date, debit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                CategoryId = 0,
                Description = "First Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = new DateOnly(),
            }
        };
        
        // Amount is 0, credit
        yield return new object[]
        {
            new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 0,
                CategoryId = 1,
                Description = "First Transaction #1",
                Type = TransactionType.Credit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
            }
        };
    }
}

public class TransactionsInsertDebitCreditBusinessTestData : TheoryData<InsertTransactionDebitCreditBusinessDataDef>
{
    public TransactionsInsertDebitCreditBusinessTestData()
    {
        // Debit with from bank allowed
        Add(new InsertTransactionDebitCreditBusinessDataDef
        {
            FromBank = 1,
            ToBank = null,
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            TransactionType = TransactionType.Debit
        });
        
        // Credit with to bank allowed
        Add(new InsertTransactionDebitCreditBusinessDataDef
        {
            FromBank = null,
            ToBank = 1,
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            TransactionType = TransactionType.Credit
        });
        
        // Credit with same from & to bank not allowed
        Add(new InsertTransactionDebitCreditBusinessDataDef
        {
            FromBank = 1,
            ToBank = 1,
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            TransactionType = TransactionType.Credit
        });
        
        // Debit with same from & to bank not allowed
        Add(new InsertTransactionDebitCreditBusinessDataDef
        {
            FromBank = 1,
            ToBank = 1,
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            TransactionType = TransactionType.Debit
        });
        
        // Debit with no from bank not allowed
        Add(new InsertTransactionDebitCreditBusinessDataDef
        {
            FromBank = null,
            ToBank = 1,
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            TransactionType = TransactionType.Debit
        });
        
        // Credit with no to bank not allowed
        Add(new InsertTransactionDebitCreditBusinessDataDef
        {
            FromBank = 1,
            ToBank = null,
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            TransactionType = TransactionType.Credit
        });
    }
}

public class TransactionsInsertSecurityEdgeCasesTestData : TheoryData<InsertTransactionSecurityEdgeCasesDataDef>
{
    public TransactionsInsertSecurityEdgeCasesTestData()
    {
        Add(new InsertTransactionSecurityEdgeCasesDataDef
        {
            Description = new string('a', 500),
            ActualAmount = 100,
            Amount = 100
        });
        
        Add(new InsertTransactionSecurityEdgeCasesDataDef
        {
            Description = "CREATE TABLE IF NOT EXISTS Injection (id INT NOT NULL)",
            ActualAmount = 100,
            Amount = 100
        });
        
        Add(new InsertTransactionSecurityEdgeCasesDataDef
        {
            Description = "</script>",
            ActualAmount = 100,
            Amount = 100
        });
        
        Add(new InsertTransactionSecurityEdgeCasesDataDef
        {
            Description = "Hello world",
            ActualAmount = 9999999999999.99m,
            Amount = 100
        });
        
        Add(new InsertTransactionSecurityEdgeCasesDataDef
        {
            Description = "Hello world",
            ActualAmount = 100,
            Amount = 9999999999999.99m
        });
    }
}

#endregion

public class TransactionsByDateInvalidOfFutureTestData : TheoryData<string>
{
    public TransactionsByDateInvalidOfFutureTestData()
    {
        Add(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10).ToString("yyyy-MM-dd"));
        Add(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10).ToString("dd-yyyy-MM"));
        Add(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10).ToString("MM-dd-yyyy"));
        Add(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10).ToString("yyyy-MM"));
        Add(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10).ToString("dd-yyyy"));
        Add(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10).ToString("dd-MM"));
        Add(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10).ToString("MM-dd"));
        Add("abcdefghijklmnop");
    }
}

public class TransactionsByMonthYearTestsData : TheoryData<TransactionsByMonthYearDataDef>
{
    public TransactionsByMonthYearTestsData()
    {
        // Check only with current month and the data should return by current month and current year
        Add(new TransactionsByMonthYearDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.OK,
            ExpectedHttpStatusCode = HttpStatusCode.OK,
            Month = DateOnly.FromDateTime(DateTime.UtcNow).Month,
            ShouldDataExists = true
        });
        
        // Check only with current year and the data should return by current month and current year
        Add(new TransactionsByMonthYearDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.OK,
            ExpectedHttpStatusCode = HttpStatusCode.OK,
            Year = DateOnly.FromDateTime(DateTime.UtcNow).Year,
            ShouldDataExists = true
        });
        
        // Check only with past month and the data should return by past month and current year
        Add(new TransactionsByMonthYearDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.OK,
            ExpectedHttpStatusCode = HttpStatusCode.OK,
            Month = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1).Month,
            ShouldDataExists = true
        });
        
        // Check only with two months back and the data should not return
        Add(new TransactionsByMonthYearDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.OK,
            ExpectedHttpStatusCode = HttpStatusCode.OK,
            Month = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-2).Month,
            ShouldDataExists = false
        });
        
        // Check only with future month and current year and the data should not return by future month and current year
        Add(new TransactionsByMonthYearDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Month = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(1).Month,
            Year = DateOnly.FromDateTime(DateTime.UtcNow).Year,
            ShouldDataExists = false
        });
        
        // Check with no month and no year and data should return with current month and current year
        Add(new TransactionsByMonthYearDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.OK,
            ExpectedHttpStatusCode = HttpStatusCode.OK,
            ShouldDataExists = true
        });
    }
}