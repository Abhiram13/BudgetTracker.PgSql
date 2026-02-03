namespace BudgetTracker.Warehouse.Interfaces;

public interface IWarehouseAppSecrets
{
    string GoogleProjectId { get; set; }

    /// <summary>
    /// Name of the Pub-Sub Subscriber for the Datewise Transactions list table in Big Query
    /// </summary>
    string DatewiseTransactionSubscriber { get; set; }
    string DataSet { get; set; }
}