using BudgetTracker.Shared.Models;
using BudgetTracker.Warehouse.Models;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Options;
using Warehouse.IntegrationTests.Model;

namespace Warehouse.IntegrationTests.Services;

public class WareHouseService
{
    private GoogleCloudProject _googleCloudProject { get; }
    private WarehouseAppSecrets _warehouseAppSecrets { get; }
    private BigQueryClient _bigQueryClient { get; }

    public WareHouseService(IOptions<GoogleCloudProject> googleCloudProject, IOptions<WarehouseAppSecrets> warehouseAppSecrets)
    {
        _googleCloudProject = googleCloudProject.Value;
        _warehouseAppSecrets = warehouseAppSecrets.Value;
        _bigQueryClient = BigQueryClient.Create(_googleCloudProject.Id);
    }

    public async Task InsertTransactionsByMonthAsync(TransactionsListByMonthDto payload)
    {
        string sql = $@"
            MERGE `{_warehouseAppSecrets.BigQuery.DataSet}.{_warehouseAppSecrets.BigQuery.Table}` T
            USING (
                SELECT
                    @date AS date,
                    @debit AS debit,
                    @credit AS credit,
                    @count AS count
                ) S
            ON T.date = S.date
            
            WHEN MATCHED THEN
                UPDATE SET debit = S.debit, credit = S.credit, count = S.count

            WHEN NOT MATCHED THEN
                INSERT (date, debit, credit, count)
                VALUES (S.date, S.debit, S.credit, S.count)
            ";

        BigQueryParameter[] parameters = new BigQueryParameter[]
        {
            new BigQueryParameter("date", BigQueryDbType.Date, payload.Date.ToString("yyyy-MM-dd")),
            new BigQueryParameter("debit", BigQueryDbType.Numeric, payload.Debit.ToString()),
            new BigQueryParameter("credit", BigQueryDbType.Numeric, payload.Credit.ToString()),
            new BigQueryParameter("count", BigQueryDbType.Int64, payload.Count.ToString()),
        };

        await _bigQueryClient.ExecuteQueryAsync(sql, parameters);
    }
}