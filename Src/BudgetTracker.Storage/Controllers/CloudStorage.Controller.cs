using BudgetTracker.Storage.Models;
using BudgetTracker.Storage.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Storage.Controllers;

[ApiController]
[Route("/api/storage")]
public class CloudStorageController : ControllerBase
{
    private readonly ILogger<CloudStorageController> _logger;
    private readonly GoogleCloudStorageService  _googleCloudStorageService;
    private readonly FileService _fileService;

    public CloudStorageController(ILogger<CloudStorageController> logger, GoogleCloudStorageService googleCloudStorageService, FileService fileService)
    {
        _logger = logger;
        _googleCloudStorageService = googleCloudStorageService;
        _fileService = fileService;
    }
    
    [HttpPost("upload")]
    public IActionResult GetSignedUrl([FromBody] UploadFileDto payload)
    {
        string extension = _fileService.GetExtension(mimeType: payload.ContentType);
        string fileName = $"{payload.FileName}_tId_{payload.TransactionId}{extension}";
        string url = _googleCloudStorageService.GenerateUploadSignedUrl(fileName: fileName);
        
        return Ok(url);
    }
}