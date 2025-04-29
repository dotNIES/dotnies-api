using CommunityToolkit.Mvvm.Messaging;
using dotNIES.API.Controllers;
using dotNIES.API.Core.Services;
using dotNIES.Data.Dto.Common;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.common.Controllers;
[Area("common")]
[Route("api/[area]/[controller]")]
[ApiController]
public class CategoryController : BaseController, ICategoryController
{
    private readonly ILoggerService _loggerService;
    private readonly IAppInfoDto _appInfoDto;
    private readonly IUserAppInfoDto _userAppInfoDto;
    private readonly IBaseDataService _baseDataService;

    public CategoryController(ILoggerService loggerService,
                              IAppInfoDto appInfoDto,
                              IUserAppInfoDto userAppInfoDto,
                              IBaseDataService baseDataService) : base(loggerService, appInfoDto, userAppInfoDto)
    {
        _loggerService = loggerService;
        _appInfoDto = appInfoDto;
        _userAppInfoDto = userAppInfoDto;
        _baseDataService = baseDataService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        try
        {
            var result = await _baseDataService.GetAllAsync<CategoryDto>("Category");

            if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
            {
                var logMessage = new LogMessageModel
                {
                    Message = $"Getting all records from Category returned {result.Count()} records",
                    LogLevel = LogLevel.Debug,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            var logMessage = new LogMessageModel
            {
                Message = $"An exception occurred while getting all records from Category",
                LogLevel = LogLevel.Error,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
            return new BadRequestResult();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetActiveCategories()
    {
        try
        {
            var result = await _baseDataService.GetDataAsync<CategoryDto>("SELECT * FROM Category WHERE IsActive = 1 AND IsDeleted = 0");

            if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
            {
                var logMessage = new LogMessageModel
                {
                    Message = $"Getting all records from Category returned {result.Count()} records",
                    LogLevel = LogLevel.Debug,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            var logMessage = new LogMessageModel
            {
                Message = $"An exception occurred while getting all records from Category",
                LogLevel = LogLevel.Error,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
            return new BadRequestResult();
        }
    }
}
