using System.Net;
using BudgetTracker.Finance.Models;
using IntegrationTests.Finance.Definations.Categories;

namespace IntegrationTests.Finance.Data.Categories;

public class InsertCategoriesTestData : TheoryData<InsertCategoryDef>
{
    public InsertCategoriesTestData()
    {
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            Payload = new InsertCategoryDto { Name = "Integration test category" },
            ShouldDataExist = true,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            Payload = new InsertCategoryDto { Name = "Integration test category #1" },
            ShouldDataExist = true,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = "1234567" },
            ShouldDataExist = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = "!@#$%^&*()" },
            ShouldDataExist = false,
        });
    }
}