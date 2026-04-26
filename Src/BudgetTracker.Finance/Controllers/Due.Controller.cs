using System.Net;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Finance.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = SharedConstants.Jwt.Policies.DOWNSTREAM_POLICY)]
[Route("api/dues")]
[Produces("application/json")]
[Consumes("application/json")]
public class DueController : ControllerBase
{
    private readonly DueService _dueService;

    public DueController(DueService dueService)
    {
        _dueService = dueService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> InsertOneAsync([FromBody] InsertDueDto payload)
    {
        await _dueService.InsertOneAsync(payload);

        return StatusCode(StatusCodes.Status201Created, new ApiResponse
        {
            StatusCode = HttpStatusCode.Created,
            Message = "Due inserted successfully",
            TraceId = ""
        });
    }
}