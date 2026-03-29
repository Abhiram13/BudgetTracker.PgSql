namespace BudgetTracker.Shared.Utilities;

public static class ValidationRegex
{
    /// <summary>
    /// Validates a description string. 
    /// Requires at least one alphabetic character and allows alphanumeric characters, 
    /// hashes (#), commas (,), and whitespace.
    /// </summary>
    /// <remarks>
    /// Pattern: <c>^(?=.*[a-zA-Z])[a-zA-Z0-9#,\s]*$</c>
    /// - Must contain at least one letter (a-z, A-Z).
    /// - Allows digits, hashes, commas, and spaces.
    /// - Prevents strings consisting only of special characters or numbers.
    /// </remarks>
    public const string DESCRIPTION_PATTERN = @"^(?=.*[a-zA-Z])[a-zA-Z0-9#,\s]*$";
    
    /// <summary>
    /// Validates a name string. 
    /// Requires at least one alphabetic character and allows alphanumeric characters, 
    /// commas (,), and whitespace.
    /// </summary>
    /// <remarks>
    /// Pattern: <c>^(?=.*[a-zA-Z])[a-zA-Z0-9,\s]*$</c>
    /// - Must contain at least one letter (a-z, A-Z).
    /// - Allows digits, commas, and spaces.
    /// - Does not allow special symbols like hashes (#).
    /// </remarks>
    public const string NAME_PATTERN = @"^(?=.*[a-zA-Z])[a-zA-Z0-9,\s]*$";
}

public static class LengthConstants
{
    public const int MAX_CATEGORY_LENGTH = 20;
    public const int MIN_CATEGORY_LENGTH = 3;
    public const int MAX_BANK_LENGTH = 25;
    public const int MIN_BANK_LENGTH = 3;
    public const int MAX_TRANSACTION_DESCRIPTION_LENGTH = 50;
    public const int MIN_TRANSACTION_DESCRIPTION_LENGTH = 3;
}