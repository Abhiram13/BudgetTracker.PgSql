namespace BudgetTracker.Finance.Interfaces;

public interface IFinanceAppSecrets
{
    string PostgresHost { get; set; }
    string PostgresDatabase { get; set; }
    string PostgresUsername { get; set; }
    string PostgresPassword { get; set; }
    string PostgresPort { get; set; }    
    string GoogleCloudProjectId { get; set; }
    string PubSubTopic { get; set; }
}