namespace UnitTests.Finance.Data.Categories;

public class InsertCategoryData : TheoryData<string>
{
    public InsertCategoryData()
    {
        Add("Travel");
        Add("Travel 1");
        Add("TRAVEL 1");
        Add("Travel ,");
        Add("Integration Test 12");
        Add("Tra");
    }
}

public class InsertCategoryInvalidTestData : TheoryData<string?>
{
    public InsertCategoryInvalidTestData()
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