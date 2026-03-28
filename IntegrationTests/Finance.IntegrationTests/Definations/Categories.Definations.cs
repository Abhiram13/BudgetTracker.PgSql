using System.Net;
using BudgetTracker.Finance.Models;

namespace IntegrationTests.Finance.Definations.Categories;

public record InsertCategoryDef
{
    public required InsertCategoryDto Payload { get; init; }
    public required HttpStatusCode ExpectedHttpStatusCode { get; init; }
    public required HttpStatusCode ExpectedApiStatusCode { get; init; }
    public required bool ShouldDataExist { get; init; }
    public required bool PreSeedData { get; init; }
    public string? ExpectedMessage { get; init; }
}