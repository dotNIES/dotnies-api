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
public class GeneralLedgerController(ILoggerService loggerService,
                                     IAppInfoDto appInfoDto,
                                     IUserAppInfoDto userAppInfoDto,
                                     IGeneralLedgerRepository generalLedgerRepository,
                                     IGeneralLedgerDetailRepository generalLedgerDetailRepository) : BaseController(loggerService, appInfoDto, userAppInfoDto)
{
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IUserAppInfoDto _userAppInfoDto = userAppInfoDto;
    private readonly IGeneralLedgerRepository _generalLedgerRepository = generalLedgerRepository;
    private readonly IGeneralLedgerDetailRepository _generalLedgerDetailRepository = generalLedgerDetailRepository;

    [HttpGet("GetGeneralLedger/{id}")]
    public async Task<ActionResult<GeneralLedgerDto?>> GetGeneralLedger(int id)
    {
        try
        {
            if (id < 1)
            {
                return BadRequest("GeneralLedgerId cannot be 0");
            }

            var result = await _generalLedgerRepository.GetGeneralLedger(id);

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting GeneralLedger with id {id} returned {result is not null}");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting the GetGeneralLedger", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetGeneralLedgers")]
    public async Task<ActionResult<IEnumerable<GeneralLedgerDto>?>> GetGeneralLedgers()
    {
        try
        {
            var result = await _generalLedgerRepository.GetAllAsync();

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from GeneralLedgers returned {result.Count()} records");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GetGeneralLedgers", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetNotDeletedGeneralLedgers")]
    public async Task<ActionResult<IEnumerable<GeneralLedgerDto>?>> GetNotDeletedGeneralLedgers()
    {
        try
        {
            var result = await _generalLedgerRepository.GetNotDeletedGeneralLedgersAsync();

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from GetNotDeletedGeneralLedgers returned {result.Count()} records");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GetNotDeletedGeneralLedgers", e);
            return new BadRequestResult();
        }
    }

    [HttpGet("GetDeletedGeneralLedgers")]
    public async Task<ActionResult<IEnumerable<GeneralLedgerDto>?>> GetDeletedGeneralLedgers()
    {
        try
        {
            var result = await _generalLedgerRepository.GetDeletedGeneralLedgersAsync();

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"Getting all records from GetDeletedGeneralLedgers returned {result.Count()} records");
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GetDeletedGeneralLedgers", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("InsertGeneralLedger/{generalLedger}")]
    public async Task<ActionResult<int>> InsertGeneralLedger(GeneralLedgerDto generalLedger)
    {
        try
        {
            if (generalLedger == null)
            {
                return BadRequest("GeneralLedgerDto cannot be null");
            }

            var result = await _generalLedgerRepository.Insert(generalLedger);

            if (result == 0)
            {
                _loggerService.SendError("An error occurred while inserting the new GeneralLedger");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedger)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed
            }

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"New GeneralLedger inserted {result > 0} with id: {result}");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in InsertGeneralLedger", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("UpdateGeneralLedger/{generalLedger}")]
    public async Task<ActionResult<bool>> UpdateGeneralLedger(GeneralLedgerDto generalLedger)
    {
        try
        {
            if (generalLedger == null)
            {
                return BadRequest("GeneralLedgerDto cannot be null");
            }

            var result = await _generalLedgerRepository.Update(generalLedger);

            if (!result)
            {
                _loggerService.SendError("An error occurred while inserting the new GeneralLedger");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedger)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed
            }

            if (_userAppInfoDto.MinimumLogLevel is LogLevel.Debug or LogLevel.Trace)
            {
                _loggerService.SendDebugInfo($"GeneralLedger {generalLedger.Id} updated: {result}");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in UpdateGeneralLedger", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("DeleteGeneralLedger/{generalLegerId}")]
    public async Task<ActionResult<bool>> DeleteGeneralLedger(int generalLedgerId)
    {
        try
        {
            if (generalLedgerId < 1)
            {
                return BadRequest("GeneralLedgerId cannot be 0");
            }

            var result = await _generalLedgerRepository.Delete(generalLedgerId);

            if (result == false)
            {
                _loggerService.SendError($"An error occurred while deleting the GeneralLedger with Id {generalLedgerId}");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in DeleteGeneralLedger", e);
            return new BadRequestResult();
        }
    }
}
