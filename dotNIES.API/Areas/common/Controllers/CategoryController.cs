using dotNIES.API.Controllers;
using dotNIES.API.Core.Repositories;
using dotNIES.API.Core.Repositories.Common;
using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Common;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.common.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController : BaseController
{
    private readonly ILoggerService _loggerService;
    private readonly IAppInfoDto _appInfoDto;
    private readonly IUserAppInfoDto _userAppInfoDto;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ILoggerService loggerService,
                              IAppInfoDto appInfoDto,
                              IUserAppInfoDto userAppInfoDto,
                              ICategoryRepository categoryRepository) : base(loggerService, appInfoDto, userAppInfoDto)
    {
        _loggerService = loggerService;
        _appInfoDto = appInfoDto;
        _userAppInfoDto = userAppInfoDto;
        _categoryRepository = categoryRepository;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        try
        {
            var result = await _categoryRepository.GetAllAsync();

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from Category returned {result.Count()} records");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from Category", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetActiveCategories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetActiveCategories()
    {
        try
        {
            var result = await _categoryRepository.GetActiveCategoriesAsync();

            if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from Category returned {result.Count()} records");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError($"An exception occurred while getting all records from Category", e);
            return new BadRequestResult();
        }
    }
}
