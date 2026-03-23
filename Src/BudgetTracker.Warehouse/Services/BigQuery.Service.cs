using System.Globalization;
using System.Text.Json;
using Abhiram.Secrets.Providers.Exceptions;
using BudgetTracker.Shared.Models;
using BudgetTracker.Warehouse.Models;
using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Warehouse.Services;

public class BigQueryService
{
    private readonly BigQueryClient _client;
    private readonly WarehouseAppSecrets _appSecrets;
    private readonly string _projectId;

    public BigQueryService(WarehouseAppSecrets appSecrets)
    {
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID") ?? throw new ProjectNotFoundException();
        _client = BigQueryClient.Create(_projectId);
        _appSecrets = appSecrets;
    }

    public async Task InsertTransactionByDateAsync([FromBody] TransactionsListByMonthDto payload)
    {
        string sql = $@"
            MERGE `{_appSecrets.BigQuery.DataSet}.{_appSecrets.BigQuery.Table}` T
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

        await _client.ExecuteQueryAsync(sql, parameters);
    }

    public async Task<List<TransactionsListByMonthDto>> GetAllTransactionsAsync(int? month, int? year)
    {
        int valueMonth = month ?? DateTime.UtcNow.Month;
        int valueYear = year ?? DateTime.UtcNow.Year;

        string query = $@"
            SELECT *
            FROM {_appSecrets.BigQuery.DataSet}.{_appSecrets.BigQuery.Table}
            WHERE EXTRACT(MONTH FROM DATE) = @month
            AND EXTRACT (YEAR FROM DATE) = @year
        ";

        BigQueryParameter[] parameters = new BigQueryParameter[]
        {
            new BigQueryParameter(name: "month", type: BigQueryDbType.Int64, value: valueMonth),
            new BigQueryParameter(name: "year", type: BigQueryDbType.Int64, value: valueYear),
        };

        BigQueryResults result = await _client.ExecuteQueryAsync(sql: query, parameters: parameters);
        List<TransactionsListByMonthDto> list = result.Select(r => new TransactionsListByMonthDto
        {
            Count = int.Parse(r["count"].ToString()!),
            Credit = decimal.Parse(r["credit"].ToString()!),
            Debit = decimal.Parse(r["debit"].ToString()!),
            Date = DateOnly.FromDateTime(
                DateTime.Parse(r["date"].ToString()!, CultureInfo.InvariantCulture)
            )
        }).ToList();

        return list;
    }
}