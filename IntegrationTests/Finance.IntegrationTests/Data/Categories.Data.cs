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
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            Payload = new InsertCategoryDto { Name = "Integration test category #1" },
            ShouldDataExist = true,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = "" },
            ShouldDataExist = false,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = null },
            ShouldDataExist = false,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = new string('a', 500) },
            ShouldDataExist = false,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = " " },
            ShouldDataExist = false,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = "1234567" },
            ShouldDataExist = false,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = "!@#$%^&*()" },
            ShouldDataExist = false,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.BadRequest,
            ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
            Payload = new InsertCategoryDto { Name = "Duplicate Test" },
            ShouldDataExist = false,
            PreSeedData = true,
        });
    }
}