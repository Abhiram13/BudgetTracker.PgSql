using System.Net;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;
using IntegrationTests.Definations.Transactions;

namespace IntegrationTests.Data.Transactions;

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
                FromBank = null,
                ToBank = 1,
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
                FromBank = 1,
                ToBank = null,
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
                FromBank = 1,
                ToBank = null,
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

#endregion