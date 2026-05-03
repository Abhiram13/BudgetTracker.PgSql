using System.Net;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Exceptions;
using IntegrationTests.Finance.Definations.Transactions;
using Microsoft.EntityFrameworkCore;

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
                ActualAmount = null,
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

public class TransactionsInsertDueMetaSuccessTestData : TheoryTestData<InsertTransactionDueIdMetaDataDef>
{
    public TransactionsInsertDueMetaSuccessTestData()
    {
        Add(new InsertTransactionDueIdMetaDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            ExpectedMetaData = true,
            Payload = new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Due Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
                DueId = 1,
            }
        });
    }
}

public class TransactionsInsertDueMetaFailureTestData : TheoryTestData<InsertTransactionDueIdMetaDataDef>
{
    public TransactionsInsertDueMetaFailureTestData()
    {
        Add(new InsertTransactionDueIdMetaDataDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            ExpectedMetaData = false,
            Payload = new InsertTransactionDto
            {
                ActualAmount = 200,
                Amount = 200,
                CategoryId = 1,
                Description = "First Invalid Due Transaction #1",
                Type = TransactionType.Debit,
                FromBank = 1,
                ToBank = null,
                Date = _currentDate,
                DueId = 10, // invalid due id
            }
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

public class TransactionsEntityValidTestData : TheoryData<Transaction>
{
    public TransactionsEntityValidTestData()
    {
        // Complete happy path values
        Add(Transaction.Create(
            actualAmount: 100,
            amount: 100,
            description: "A Sample Description",
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only Max amount
        Add(Transaction.Create(
            actualAmount: 1,
            amount: 1_000_000m,
            description: "A Sample Description",
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only Min amount
        Add(Transaction.Create(
            actualAmount: 1,
            amount: 0.01m,
            description: "A Sample Description",
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        
        // Only Max actual amount
        Add(Transaction.Create(
            actualAmount: 1_000_000m,
            amount: 100,
            description: "A Sample Description",
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only Min actual amount
        Add(Transaction.Create(
            actualAmount: 0.01m,
            amount: 100,
            description: "A Sample Description",
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only Null actual amount
        Add(Transaction.Create(
            actualAmount: null,
            amount: 100,
            description: "A Sample Description",
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only max description limit
        Add(Transaction.Create(
            actualAmount: 1_000_000m,
            amount: 100,
            description: new string('a', 50),
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only min description limit
        Add(Transaction.Create(
            actualAmount: 1_000_000m,
            amount: 100,
            description: new string('a', 3),
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only description with comma and numbers and #
        Add(Transaction.Create(
            actualAmount: 1_000_000m,
            amount: 100,
            description: "Sample description #1, #2",
            categoryId: 1,
            fromBank: 1,
            toBank: null,
            type: TransactionType.Debit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
        
        // Only credit type with to bank
        Add(Transaction.Create(
            actualAmount: 1_000_000m,
            amount: 100,
            description: new string('a', 50),
            categoryId: 1,
            fromBank: null,
            toBank: 1,
            type: TransactionType.Credit,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        ));
    }
}

public class TransactionsEntityInValidTestData : TheoryData<InsertTransactionInvalidEntityThrowsExceptionDto>
{
    public TransactionsEntityInValidTestData()
    {
        // Actual amount is above limit
        // Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        // {
        //     Payload = new InsertTransactionDto
        //     {
        //         ActualAmount = 1_000_001m,
        //         Amount = 100,
        //         Description = "A Sample Description",
        //         CategoryId = 1,
        //         FromBank = 1,
        //         ToBank = null,
        //         Type = TransactionType.Debit,
        //         Date = DateOnly.FromDateTime(DateTime.UtcNow),
        //     },
        //     ExpectedExceptionType = typeof(InvalidPayloadException),
        // });
        
        // Amount is above limit
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 100,
                Amount = 1_000_001m,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Amount is zero
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 100,
                Amount = 0,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Amount is not given 
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Less than min length description
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                Description = "A",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Less than min length description
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                Description = "Ab",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Empty description
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                Description = "",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Spaces description
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                Description = " ",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Spaces description
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                Description = "   ",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // Null description
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                Description = null,
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        // numbers description
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 0,
                Amount = 100,
                Description = "1234567",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(InvalidPayloadException),
        });
        
        const string SPECIAL_CHARS = "!@$%^&*()-_+={}[]\\|;:'?/><.~`";

        foreach (char c in SPECIAL_CHARS)
        {
            Add(new InsertTransactionInvalidEntityThrowsExceptionDto
            {
                Payload = new InsertTransactionDto
                {
                    ActualAmount = 100,
                    Amount = 100,
                    Description = $"A Sample Description {c}",
                    CategoryId = 1,
                    FromBank = 1,
                    ToBank = null,
                    Type = TransactionType.Debit,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                },
                ExpectedExceptionType = typeof(InvalidPayloadException),
            });
        }
        
        // Invalid category id
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 0,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Invalid category id
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 100,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Invalid from bank id
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 0,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Invalid from bank id
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 100,
                ToBank = null,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Invalid to bank id
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = null,
                ToBank = 0,
                Type = TransactionType.Credit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Invalid to bank id
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = null,
                ToBank = 100,
                Type = TransactionType.Credit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Invalid to bank and from bank id with credit type
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 10,
                ToBank = 100,
                Type = TransactionType.Credit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Invalid to bank and from bank id with debit type
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 10,
                ToBank = 100,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Same to bank and from bank id with debit type
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 10,
                ToBank = 10,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // Same to bank and from bank id with credit type
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 10,
                ToBank = 10,
                Type = TransactionType.Credit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // To bank with debit type
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = null,
                ToBank = 1,
                Type = TransactionType.Debit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
        
        // From bank with Credit type
        Add(new InsertTransactionInvalidEntityThrowsExceptionDto
        {
            Payload = new InsertTransactionDto
            {
                ActualAmount = 10,
                Amount = 100,
                Description = "A Sample Description",
                CategoryId = 1,
                FromBank = 1,
                ToBank = null,
                Type = TransactionType.Credit,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
            },
            ExpectedExceptionType = typeof(DbUpdateException),
        });
    }
}