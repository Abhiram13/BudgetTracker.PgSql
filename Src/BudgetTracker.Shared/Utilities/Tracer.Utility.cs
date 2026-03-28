namespace BudgetTracker.Shared.Utilities;

// [Obsolete(message: "Use System.Diagnostics.Activity instead")] // TODO: Removing this "Obselete" for now as to focus on main work.
public class TraceIdProvider
{
    public string TraceId { get { return Guid.NewGuid().ToString(); } }
}