namespace IntegrationTests.Finance.Data.Banks;

public class BankEntityValidTestData : TheoryData<string>
{
    public BankEntityValidTestData()
    {
        Add("Test Bank");
        Add("Test Bank 1");
        Add("TEST BANK 1");
        Add("Test Bank ,");
        Add("Integration Test 12");
        Add("Tes");
    }
}

public class BankEntityInValidTestData : TheoryData<string?>
{
    public BankEntityInValidTestData()
    {
        Add("Te");
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
            Add($"Test Bank {c}");
        }
    }
}