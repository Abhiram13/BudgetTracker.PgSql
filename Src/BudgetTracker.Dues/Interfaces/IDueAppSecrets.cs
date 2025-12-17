namespace BudgetTracker.Dues.Interfaces;

public interface IDueAppSecrets
{
    string PostgresHost { get; set; }
    string PostgresDatabase { get; set; }
    string PostgresUsername { get; set; }
    string PostgresPassword { get; set; }
    string PostgresPort { get; set; }
}