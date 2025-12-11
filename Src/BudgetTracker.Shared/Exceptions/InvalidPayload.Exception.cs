namespace BudgetTracker.Shared.Exceptions;

public class InvalidPayloadException : Exception
{
    public InvalidPayloadException(string Message) : base (Message) { }
}