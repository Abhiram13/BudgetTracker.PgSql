using Yarp.ReverseProxy.Configuration;

namespace BudgetTracker.Gateway.Config;

public static class GatewayConfiguration
{
    public static RouteConfig[] Routes = new RouteConfig[]
    {
        new RouteConfig
        {
            RouteId = "finance-transaction-route",
            ClusterId = "finance-cluster",
            Match = new RouteMatch { Path = "/api/transactions/{**catch-all}" }
        },
        new RouteConfig
        {
            RouteId = "finance-category-route",
            ClusterId = "finance-cluster",
            Match = new RouteMatch { Path = "/api/categories/{**catch-all}" }
        },
        new RouteConfig
        {
            RouteId = "finance-bank-route",
            ClusterId = "finance-cluster",
            Match = new RouteMatch { Path = "/api/banks/{**catch-all}" }
        },
        new RouteConfig
        {
            RouteId = "warehouse-route",
            ClusterId = "warehouse-cluster",
            Match = new RouteMatch { Path = "/api/query/{**catch-all}" }
        },
        new RouteConfig
        {
            RouteId = "dues-route",
            ClusterId = "dues-cluster",
            Match = new RouteMatch { Path = "/api/dues/{**catch-all}" }
        }
    };

    public static ClusterConfig[] Clusters = new ClusterConfig[]
    {
        new ClusterConfig
        {
            ClusterId = "finance-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["instance-01"] = new DestinationConfig { Address = Environment.GetEnvironmentVariable("FINANCE_SERVICE_URL")! }
            }
        },
        new ClusterConfig
        {
            ClusterId = "warehouse-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["instance-01"] = new DestinationConfig { Address = Environment.GetEnvironmentVariable("WAREHOUSE_SERVICE_URL")! }
            }
        },
        new ClusterConfig
        {
            ClusterId = "dues-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["instance-01"] = new DestinationConfig { Address = Environment.GetEnvironmentVariable("DUES_SERVICE_URL")! }
            }
        }
    };
}