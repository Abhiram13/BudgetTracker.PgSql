namespace BudgetTracker.Shared.Utilities;

[Obsolete(message: "Use System.Diagnostics.Activity instead")]
public class TraceIdProvider
{
    public string TraceId { get { return Guid.NewGuid().ToString(); } }
}