using System.Net;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Models;

namespace IntegrationTests.Finance.Definations.Transactions;

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

/// <summary>
/// Defination used to verify transaction insertion with invalid or valid due id along with <see cref="TransactionsMeta"/>
/// </summary>
public record InsertTransactionDueIdMetaDataDef
{
    public required HttpStatusCode ExpectedHttpStatusCode { get; init; }
    public required HttpStatusCode ExpectedApiStatusCode { get; init; }
    public bool ExpectedMetaData { get; init; } = true;
    public required InsertTransactionDto Payload { get; init; }
}

public record InsertTransactionInvalidEntityThrowsExceptionDto
{
    public required InsertTransactionDto Payload { get; init; }
    public required Type ExpectedExceptionType { get; init; }
}

public record TransactionsDateWiseListDto
{
    public required Transaction[] Transactions { get; init; }
    public int? Month { get; init; }
    public int? Year { get; init; }
    public required List<TransactionsListByMonthYear> ExpectedResult { get; init; }
}
