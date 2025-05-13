using dotNIES.API.Controllers;
using dotNIES.API.Core.Repositories.Finance;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.fin.Controllers;
[Area("fin")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize]
public class PurchaseTypeController(ILoggerService loggerService,
                              IAppInfoDto appInfoDto,
                              IUserAppInfoDto userAppInfoDto,
                              IPurchaseTypeRepository purchaseTypeRepository) : BaseController(loggerService, appInfoDto, userAppInfoDto)
{
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IUserAppInfoDto _userAppInfoDto = userAppInfoDto;
    private readonly IPurchaseTypeRepository _purchaseTypeRepository = purchaseTypeRepository;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<PurchaseTypeDto>>> GetAll()
    {
        try
        {
            var result = await _purchaseTypeRepository.GetAllAsync();

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from PurchaseType returned {result.Count()} records");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PurchaseType", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetActivePurchaseTypes")]
    public async Task<ActionResult<IEnumerable<PurchaseTypeDto>>> GetActivePurchaseTypes()
    {
        try
        {
            var result = await _purchaseTypeRepository.GetActivePurchaseTypesAsync();

            if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from PurchaseType returned {result.Count()} records");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError($"An exception occurred while getting all records from PurchaseType", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetSinglePurchaseType/{id}")]
    public async Task<ActionResult<PurchaseTypeDto>> GetSinglePurchaseType(int id)
    {
        try
        {
            var result = await _purchaseTypeRepository.GetPurchaseTypeByIdAsync(id);

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting PurchaseType with Id {id} returned a record: {result is not null}");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PurchaseType", e);
            return new BadRequestResult();
        }
    }
}