using System.Net;

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

