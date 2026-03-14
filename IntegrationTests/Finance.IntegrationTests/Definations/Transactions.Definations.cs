using System.Net;
using BudgetTracker.Finance.Enums;

namespace IntegrationTests.Definations.Transactions;

/// <summary>
/// Test definations to test Date validations when inserting transactions
/// </summary>
public record InsertTransactionDateDef
{
    public required DateOnly Date { get; init; }
    public required HttpStatusCode ExpectedHttpStatusCode { get; init; }
    public required HttpStatusCode ExpectedApiStatusCode { get; init; }
    public required bool ShouldDataExists { get; init; }
}

public record InsertTransactionDebitCreditBusinessDataDef
{
    public required TransactionType TransactionType { get; init; }
    public required int? FromBank { get; init; }
    public required int? ToBank { get; init; }
    public required HttpStatusCode ExpectedHttpStatusCode { get; init; }
    public required HttpStatusCode ExpectedApiStatusCode { get; init; }
}

public record InsertTransactionSecurityEdgeCasesDataDef
{
    public required string Description { get; init; }
    public required decimal Amount { get; init; }
    public required decimal ActualAmount { get; init; }
}

public record TransactionsByMonthYearDataDef
{
    public int? Month { get; init; }
    public int? Year { get; init; }
    public required bool ShouldDataExists { get; init; }
    public required HttpStatusCode ExpectedHttpStatusCode { get; init; }
    public required HttpStatusCode ExpectedApiStatusCode { get; init; }
}

