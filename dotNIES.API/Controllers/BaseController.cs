using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    private readonly ILoggerService _loggerService;
    private readonly IAppInfoDto _appInfoDto;
    private readonly IUserAppInfoDto _userAppInfoDto;

    public BaseController(ILoggerService loggerService, IAppInfoDto appInfoDto, IUserAppInfoDto userAppInfoDto)
    {
        _loggerService = loggerService;
        _appInfoDto = appInfoDto;
        _userAppInfoDto = userAppInfoDto;
    }
}
