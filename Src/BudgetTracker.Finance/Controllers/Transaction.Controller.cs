using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Exceptions;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using BudgetTracker.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Annotations;

namespace BudgetTracker.Finance.Controllers;

/// <summary>
/// Provides endpoints for managing and retrieving transactions.
/// </summary>
/// <remarks>
/// All requests to this controller require a valid <c>YARP_API_KEY</c> provided in the request headers.
/// </remarks>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = SharedConstants.Jwt.Policies.DOWNSTREAM_POLICY)]
[Route("api/transactions")]
[Produces("application/json")]
[Consumes("application/json")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;
    private readonly TraceIdProvider _traceProvider;

    /// <summary>
    /// Initializes the controller that handles all incoming web requests for transaction APIs.
    /// </summary>
    /// <param name="transactionService"><see cref="TransactionService"/> - The service that handles the actual business logic.</param>
    /// <param name="trace"><see cref="TraceIdProvider"/> - A provider that gives a unique GUID for each request, used to track logs and find bugs.</param>
    public TransactionController(TransactionService transactionService, TraceIdProvider trace)
    {
        _transactionService = transactionService;
        _traceProvider = trace;
    }

    /// <summary>
    /// Inserts a new transaction.
    /// </summary>
    /// <param name="payload">The transaction details including amount, source, and metadata.</param>
    /// <returns>A response containing the ID and status of the inserted transaction.</returns>
    /// <response code="201">Returns the created transaction details</response>
    /// <response code="400">
    /// Returns when Payload is invalid like, date is future or in invalid format, amount is not in range or invalid bank or category ID provided.
    /// </response>
    /// <response code="500">
    /// Returns when any internal exception or DB updates failed due to constraints violations.
    /// </response>
    /// <exception cref="InvalidPayloadException">
    /// Thrown when <paramref name="payload"/> fails validation (e.g., negative amounts, missing required fields, future dates).
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// Thrown if the transaction cannot be inserted in the database, due to any constraint violations
    /// </exception>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InsertTransactionResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<InsertTransactionResponseDto>>> InsertAsync([FromBody] InsertTransactionDto payload)
    {
        InsertTransactionResponseDto response = await _transactionService.InsertTransactionAsync(payload);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<InsertTransactionResponseDto>
        {
            StatusCode = System.Net.HttpStatusCode.Created,
            TraceId = _traceProvider.TraceId,
            Message = "Transaction created successfully",
            Result = response
        });
    }
    
    /// <summary>
    /// Retrieves all transactions recorded on a specific date. Can return empty <see cref="TransactionByDateDto"/> if none found.
    /// </summary>
    /// <param name="date">The date string (e.g., <c>yyyy-MM-dd</c>) to filter.</param>
    /// <exception cref="InvalidDateException">Thrown when given date is not in <c>yyyy-MM-dd</c> format or if future date is given</exception>
    /// <response code="200">Returns list of transactions based on given date</response>
    /// <response code="400">Returns when date is in invalid format or future.</response>
    /// <response code="500">Returns when any internal exception or DB updates failed due to constraints violations.</response>
    [HttpGet("date/{date}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionByDateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<TransactionByDateDto>>> GetTransactionsByDateAsync([FromRoute, Required] string date)
    {
        TransactionByDateDto result = await _transactionService.GetTransactionsByDateAsync(date);
        return Ok(new ApiResponse<TransactionByDateDto>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = result
        });
    }

    /// <summary>
    /// Counts total transactions for given month and year
    /// </summary>
    /// <param name="month">Optional month (1-12) to filter the count. Defaults to current month</param>
    /// <param name="year">Optional year to filter the count. Defaults to current year</param>
    /// <exception cref="InvalidPayloadException">Thrown when given month or year is greater than current month and year</exception>
    /// <response code="200">Returns total count of transactions based on given month and year</response>
    /// <response code="400">Returns when given month or year is in invalid format or future.</response>
    /// <response code="500">Returns when any internal exception or DB updates failed due to constraints violations.</response>
    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<TransactionByDateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CountOfTransactionsAsync([FromQuery] int? month, [FromQuery] int? year)
    {
        int count = await _transactionService.CountOfAllTransactionsAsync(month, year);

        return Ok(new ApiResponse<int>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Result = count
        });
    }

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <remarks>All the values provided will be replaced</remarks>
    /// <param name="payload">The <see cref="UpdateTransactionDto"/> containing updated values</param>
    /// <param name="id">The ID of the transaction to update.</param>
    /// <exception cref="BadHttpRequestException">Thrown if no transaction exists with the provided <paramref name="id"/>.</exception>
    /// <response code="200">Returns when a transaction is successfully updated</response>
    /// <response code="400">Returns any of the payload values are invalid like date is in wrong format or category, bank ids are invalid.</response>
    /// <response code="500">Returns when any internal exception or DB updates failed due to constraints violations.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionByDateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTransactionAsync([FromRoute] int id, [FromBody] UpdateTransactionDto payload)
    {
        await _transactionService.UpdateTransactionAsync(payload, id);
        
        return Ok(new ApiResponse
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            TraceId = _traceProvider.TraceId,
            Message = "Transaction updated successfully"
        });
    }

    // [HttpGet("bigQuery")]
    // public async Task<IActionResult> BigQueryUpdatesAsync()
    // {
    //     await _transactionService.BigQueryUpdatesAsync();
    //     
    //     return Ok(new  ApiResponse<string>
    //     {
    //         StatusCode = System.Net.HttpStatusCode.OK,
    //         TraceId = _traceProvider.TraceId,
    //         Message = "Big query updated successfully"
    //     });
    // }
}