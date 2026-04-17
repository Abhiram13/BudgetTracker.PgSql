namespace BudgetTracker.Shared.Constants;

public static partial class SharedConstants
{
    /// <summary>
    /// Provides a centralized collection of regular expressions for data validation.
    /// </summary>
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
}