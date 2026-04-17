namespace BudgetTracker.Shared.Constants;

public static class JwtConstants
{
    public const string Issuer = "ApiGateway";

    public static class Audience
    {
        public const string FinanceCluster = "finance-cluster";
        public const string WarehouseCluster = "warehouse-cluster";
    }
    
    public static class Scopes
    {
        public const string DOWNSTREAM = "downstream";
        public const string INTERNAL = "internal";
    }
    
    public static class Policies
    {
        public const string DOWNSTREAM_POLICY = "DownstreamPolicy";
        public const string INTERNAL_POLICY = "InternalPolicy";
    }
}