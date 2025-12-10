using Microsoft.AspNetCore.Mvc;

namespace PostgresWebApiTemplate.Controllers;

[ApiController]
[Route("")]
public class InitController : BaseApiController
{
    private readonly ILogger<InitController> _logger;

    public InitController(ILogger<InitController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Hello");
        return Ok("Hello. This is Postgres Template");
    }
}