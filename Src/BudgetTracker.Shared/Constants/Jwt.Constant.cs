namespace BudgetTracker.Shared.Constants;

public static partial class SharedConstants
{
    public static class Jwt
    {
        public const string Issuer = "ApiGateway";
    
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
}