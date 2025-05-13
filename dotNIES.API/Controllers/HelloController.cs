using CommunityToolkit.Mvvm.Messaging;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Messages;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Controllers;
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class HelloController : BaseController
{
    private readonly ILoggerService _loggerService;

    public HelloController(ILoggerService loggerService, IAppInfoDto appInfoDto, IUserAppInfoDto userAppInfoDto)
        : base(loggerService, appInfoDto, userAppInfoDto)
    {
        _loggerService = loggerService;
    }

    /// <summary>
    /// Say hello to the world
    /// </summary>
    /// <returns></returns>
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
