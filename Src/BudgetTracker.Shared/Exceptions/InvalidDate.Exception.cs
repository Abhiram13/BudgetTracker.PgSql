namespace BudgetTracker.Shared.Exceptions;

public class InvalidDateException : Exception
{
    public InvalidDateException(string Message) : base (Message) { }
    
    public InvalidDateException() : base(message: "Invalid transaction date specified.") { } 
}