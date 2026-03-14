using Google.Cloud.BigQuery.V2;

namespace Warehouse.IntegrationTests;

public static class Constants
{
    public static readonly string PROJECT_ID = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID")!;
    public static readonly string DATASET = "testing";
    public static readonly string TABLE = "transactions_by_month";
}