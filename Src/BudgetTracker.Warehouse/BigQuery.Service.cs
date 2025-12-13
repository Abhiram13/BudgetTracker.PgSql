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
        // delete existing record first
        // string deleteQuery = @"
        //     DELETE FROM budgettracker.transactions_by_date
        //     WHERE date = DATE(@date);
        // ";

        // await _client.ExecuteQueryAsync(sql: deleteQuery, parameters: new[]
        // {
        //     new BigQueryParameter(name: "date", type: BigQueryDbType.Date, value: payload.Date.ToString("yyyy-MM-dd"))
        // });

        // then insert new record in it place
        BigQueryInsertResults result = await _client.InsertRowAsync(
            datasetId: "budgettracker",
            tableId: "transactions_by_date",
            row: new BigQueryInsertRow
            {
                {"date", payload.Date.ToString("yyyy-MM-dd")},
                {"debit", payload.Debit},
                {"credit", payload.Credit}
            }
        );
    }
}