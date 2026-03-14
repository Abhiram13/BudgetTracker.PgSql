using BudgetTracker.Shared.Models;
using Google.Cloud.BigQuery.V2;
using Warehouse.IntegrationTests.Setup;

namespace Warehouse.IntegrationTests.Services;

public class WareHouseService
{
    private readonly BigQueryClient _bigQueryClient = BigQueryClient.Create(Constants.PROJECT_ID);

    public async Task InsertTransactionsByMonthAsync(TransactionsListByMonthDto payload)
    {
        string sql = $@"
            MERGE `{Constants.DATASET}.{Constants.TABLE}` T
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