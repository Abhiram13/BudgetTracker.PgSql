using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Models;

namespace BudgetTracker.Warehouse.Models;

public record WarehouseAppSecrets
{
    public YarpApiKeySecret Secrets { get; set; } =  default!;
    public BigQuerySecrets BigQuery { get; set; } = default!;
}

public record BigQuerySecrets
{
    [ConfigurationKeyName("DATASET")]
    public string DataSet { get; init; } = string.Empty;
    
    [ConfigurationKeyName("PUBSUB_DATEWISE_TRANSACTIONS_SUBSCRIBER")]
    public string DateWiseTransactionSubscriber { get; init; } = string.Empty;
    
    [ConfigurationKeyName("TABLE")]
    public string Table { get; init; } = string.Empty;
    
    [ConfigurationKeyName("METADATA_TABLE")]
    public string MetadataTable { get; init; } = string.Empty;
}