namespace BudgetTracker.Shared.Utilities;

public class TraceIdProvider
{
    public string TraceId { get { return Guid.NewGuid().ToString(); } }
}