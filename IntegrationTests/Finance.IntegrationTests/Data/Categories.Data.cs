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
            Payload = new InsertCategoryDto { Name = "Integration test" },
            ShouldDataExist = true,
            PreSeedData = false,
        });
        
        Add(new InsertCategoryDef
        {
            ExpectedApiStatusCode = HttpStatusCode.Created,
            ExpectedHttpStatusCode = HttpStatusCode.Created,
            Payload = new InsertCategoryDto { Name = "Integration test 1" },
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
            #pragma warning disable CS8625 
            
            Payload = new InsertCategoryDto { Name = null },
            
            #pragma warning restore CS8625 
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

public class CategoryEntityValidTestData : TheoryData<string>
{
    public CategoryEntityValidTestData()
    {
        Add("Travel");
        Add("Travel 1");
        Add("TRAVEL 1");
        Add("Travel ,");
        Add("Integration Test 12");
        Add("Tra");
    }
}

public class CategoryEntityInValidTestData : TheoryData<string?>
{
    public CategoryEntityInValidTestData()
    {
        Add("Tr");
        Add("T");
        Add("");
        Add(" ");
        Add("     ");
        Add(null);
        Add("12345");
        Add("1");
        Add("t");
        
        const string SPECIAL_CHARS = "!@#$%^&*()-_+={}[]\\|;:'?/><.~`";
        foreach (char c in SPECIAL_CHARS)
        {
            Add($"Travel {c}");
        }
    }
}