using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Warehouse.Interfaces;

namespace BudgetTracker.Warehouse.Models;

public sealed class WarehouseAppSecrets : IWarehouseAppSecrets, IYarpApiKeyAppSecret
{
    public string GoogleProjectId { get; set; } = string.Empty;
    public string DatewiseTransactionSubscriber { get; set; } = string.Empty;
    public string YarpApiKey { get; set; } = string.Empty;
}