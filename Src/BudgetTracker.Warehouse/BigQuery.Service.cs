using System.Text.Json;
using BudgetTracker.Shared.Models;
using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Warehouse.Services;

public class BigQueryService
{
    private readonly BigQueryClient _client;
    private readonly string _projectId = "budget-tracker-453204";

    public BigQueryService()
    {
        _client = BigQueryClient.Create(_projectId);
    }

    public async Task InsertTransactionByDateAsync([FromBody] DateTransactionsDto payload)
    {
        string sql = @"
            MERGE `budgettracker.transactions_by_date` T
            USING (
                SELECT
                    @date   AS date,
                    @debit  AS debit,
                    @credit AS credit
                ) S
            ON T.date = S.date
            
            WHEN MATCHED THEN
                UPDATE SET debit  = S.debit, credit = S.credit

            WHEN NOT MATCHED THEN
                INSERT (date, debit, credit)
                VALUES (S.date, S.debit, S.credit)
            ";

        BigQueryParameter[] parameters = new BigQueryParameter[]
        {
            new BigQueryParameter("date", BigQueryDbType.Date, payload.Date.ToString("yyyy-MM-dd")),
            new BigQueryParameter("debit", BigQueryDbType.Numeric, payload.Debit.ToString()),
            new BigQueryParameter("credit", BigQueryDbType.Numeric, payload.Credit.ToString())
        };

        await _client.ExecuteQueryAsync(sql, parameters);
    }

    public async Task<List<DateTransactionsDto>> GetAllTransactionsAsync(int? month, int? year)
    {
        int valueMonth = month ?? DateTime.UtcNow.Month;
        int valueYear = year ?? DateTime.UtcNow.Year;

        string query = @"
            SELECT *
            FROM budgettracker.transactions_by_date
            WHERE EXTRACT(MONTH FROM DATE) = @month
            AND EXTRACT (YEAR FROM DATE) = @year
        ";

        BigQueryParameter[] parameters = new BigQueryParameter[]
        {
            new BigQueryParameter(name: "month", type: BigQueryDbType.Int64, value: valueMonth),
            new BigQueryParameter(name: "year", type: BigQueryDbType.Int64, value: valueYear),
        };

        BigQueryResults result = await _client.ExecuteQueryAsync(sql: query, parameters: parameters);
        List<DateTransactionsDto> list = result.Select(r => new DateTransactionsDto
        {
            Credit = double.Parse(r["credit"].ToString()!),
            Debit = double.Parse(r["debit"].ToString()!),
            Date = DateTime.Parse(r["date"].ToString()!)
        }).ToList();

        return list;
    }
}