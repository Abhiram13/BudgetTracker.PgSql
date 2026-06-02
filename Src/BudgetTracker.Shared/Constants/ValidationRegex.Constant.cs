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
        /// Allows indian languages and english.
        /// Used in Transactions description
        /// </summary>
        public const string DESCRIPTION_PATTERN = @"^[\p{L}\p{M}\p{N} #]+$";
    
        /// <summary>
        /// Validates a name string. 
        /// Requires at least one alphabetic character and allows alphanumeric characters, 
        /// commas (,), and whitespace. Used in Category, Bank names
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