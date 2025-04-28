using CommunityToolkit.Mvvm.Messaging;
using dotNIES.Data.Logging.Messages;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    private readonly ILoggerService _loggerService;

    public HelloController(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var logMessage = new LogMessageModel
        {
            Message = "Hello from dotNIES API",
            LogLevel = LogLevel.Information
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
        return Ok("Hello from dotNIES API :-)");
    }
}
