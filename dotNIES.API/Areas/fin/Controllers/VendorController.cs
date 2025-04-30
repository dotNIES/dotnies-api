using dotNIES.API.Controllers;
using dotNIES.API.Core.Repositories;
using dotNIES.Data.Dto.Common;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.fin.Controllers;

[Area("fin")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize]
public class VendorController : BaseController
{
    private readonly ILoggerService _loggerService;
    private readonly IAppInfoDto _appInfoDto;
    private readonly IUserAppInfoDto _userAppInfoDto;
    private readonly IVendorRepository _vendorRepository;

    public VendorController(ILoggerService loggerService,
                              IAppInfoDto appInfoDto,
                              IUserAppInfoDto userAppInfoDto,
                              IVendorRepository vendorRepository) : base(loggerService, appInfoDto, userAppInfoDto)
    {
        _loggerService = loggerService;
        _appInfoDto = appInfoDto;
        _userAppInfoDto = userAppInfoDto;
        _vendorRepository = vendorRepository;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<VendorDto>>> GetAll()
    {
        try
        {
            var result = await _vendorRepository.GetAllAsync();

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from Vendor returned {result.Count()} records");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from Vendor", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetActiveVendors")]
    public async Task<ActionResult<IEnumerable<VendorDto>>> GetActiveVendors()
    {
        try
        {
            var result = await _vendorRepository.GetActiveVendorsAsync();

            if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from Vendor returned {result.Count()} records");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError($"An exception occurred while getting all records from Vendor", e);
            return new BadRequestResult();
        }
    }
}
