using BudgetTracker.Finance.Models;

namespace IntegrationTests.Finance.Data.Dues;

public class DuesEntityValidTestData : TheoryData<InsertDueDto>
{
    public DuesEntityValidTestData()
    {
        Add(new InsertDueDto
        {
            Description = "Sample Description",
            Title =  "Sample Title",
            TotalAmount = 100,
            Comments =  "Sample comments",
            Creditor = "John",
            Debtor =  "Jane",
            Remarks =  "Sample remarks",
            StartDate = DateOnly.FromDateTime(DateTime.Now),
        });
    }
}