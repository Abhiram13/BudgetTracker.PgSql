namespace BudgetTracker.Shared.Constants;

public static partial class SharedConstants
{
    /// <summary>
    /// Defines character length constraints for different data fields.
    /// </summary>
    public static class LengthConstants
    {
        /// <summary>Maximum allowed characters for a Category name.</summary>
        /// <remarks><c>20</c></remarks>
        public const int MAX_CATEGORY_LENGTH = 20;
    
        /// <summary>Minimum allowed characters for a Category name.</summary>
        /// <remarks><c>3</c></remarks>
        public const int MIN_CATEGORY_LENGTH = 3;
    
        /// <summary>Maximum allowed characters for Bank name.</summary>
        /// <remarks><c>25</c></remarks>
        public const int MAX_BANK_LENGTH = 25;
    
        /// <summary>Minimum allowed characters for Bank name.</summary>
        /// <remarks><c>3</c></remarks>
        public const int MIN_BANK_LENGTH = 3;
    
        /// <summary>Maximum allowed characters for Transaction description.</summary>
        /// <remarks><c>50</c></remarks>
        public const int MAX_TRANSACTION_DESCRIPTION_LENGTH = 50;
    
        /// <summary>Minimum allowed characters for Transaction description.</summary>
        /// <remarks><c>3</c></remarks>
        public const int MIN_TRANSACTION_DESCRIPTION_LENGTH = 3;
    }
}