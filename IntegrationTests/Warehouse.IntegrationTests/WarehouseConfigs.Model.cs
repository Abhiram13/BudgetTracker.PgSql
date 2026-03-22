using Microsoft.Extensions.Configuration;

namespace Warehouse.IntegrationTests.Model;

public record WareHouseConfiguration
{
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public string GoogleCloudProjectId { get; init; } = string.Empty;
    
    [ConfigurationKeyName("DataSet")]
    public string DataSet { get; init; } = string.Empty;
    
    [ConfigurationKeyName("Table")]
    public string Table { get; init; } = string.Empty;
    
    [ConfigurationKeyName("YarpApiKey")]
    public string YarpApiKey { get; init; } = string.Empty;
}