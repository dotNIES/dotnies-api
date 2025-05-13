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
public class PaymentTypeController(ILoggerService loggerService,
                                   IAppInfoDto appInfoDto,
                                   IUserAppInfoDto userAppInfoDto,
                                   IPaymentTypeRepository paymentTypeRepository) : BaseController(loggerService, appInfoDto, userAppInfoDto)
{
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IUserAppInfoDto _userAppInfoDto = userAppInfoDto;
    private readonly IPaymentTypeRepository _paymentTypeRepository = paymentTypeRepository;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<PaymentTypeDto>>> GetAll()
    {
        try
        {
            var result = await _paymentTypeRepository.GetAllAsync();

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from PaymentType returned {result.Count()} records");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PaymentType", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetActivePaymentTypes")]
    public async Task<ActionResult<IEnumerable<PaymentTypeDto>>> GetActivePaymentTypes()
    {
        try
        {
            var result = await _paymentTypeRepository.GetActivePaymentTypesAsync();

            if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from PaymentType returned {result.Count()} records");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError($"An exception occurred while getting all records from PaymentType", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetSinglePaymentType/{id}")]
    public async Task<ActionResult<PaymentTypeDto>> GetSinglePaymentType(int id)
    {
        try
        {
            var result = await _paymentTypeRepository.GetPaymentTypeByIdAsync(id);

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting PaymentType with Id {id} returned a record: {result is not null}");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PaymentType", e);
            return new BadRequestResult();
        }
    }
}
