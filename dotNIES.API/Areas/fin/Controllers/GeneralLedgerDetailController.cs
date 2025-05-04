using dotNIES.API.Controllers;
using dotNIES.API.Core.Repositories.Finance;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.fin.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GeneralLedgerDetailController(ILoggerService loggerService,
                                           IAppInfoDto appInfoDto,
                                           IUserAppInfoDto userAppInfoDto,
                                           IGeneralLedgerDetailRepository generalLedgerDetailRepository) : BaseController(loggerService, appInfoDto, userAppInfoDto)
{
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IGeneralLedgerDetailRepository _generalLedgerDetailRepository = generalLedgerDetailRepository;

    [HttpPost("GetGeneralLedgerDetails/{generalLedgerId}")]
    public async Task<ActionResult<IEnumerable<GeneralLedgerDetailDto>?>> GetGeneralLedgerDetails(int generalLedgerId)
    {
        try
        {
            if (generalLedgerId < 1)
            {
                return BadRequest("GeneralLedgerId cannot be 0");
            }

            var result = await _generalLedgerDetailRepository.GetGeneralLedgerDetailsForGL(generalLedgerId);

            _loggerService.SendDebugInfo($"Getting all records from GetGeneralLedgerDetails for GeneralLedgerId {generalLedgerId} returned {result?.Count()} records");

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GetGeneralLedgerDetails", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("InsertGeneralLedgerDetail/{generalLedgerDetail}")]
    public async Task<ActionResult<int>> InsertGeneralLedgerDetail(GeneralLedgerDetailDto generalLedgerDetail)
    {
        try
        {
            if (generalLedgerDetail == null)
            {
                return BadRequest("GeneralLedgerDetailDto cannot be null");
            }

            var result = await _generalLedgerDetailRepository.Insert(generalLedgerDetail);
            
            _loggerService.SendDebugInfo($"Inserting GeneralLedgerDetailDto: {result > 0}");

            return Ok(result);
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in InsertGeneralLedgerDetail", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("UpdateGeneralLedgerDetail/{generalLedgerDetail}")]
    public async Task<ActionResult<bool>> UpdateGeneralLedgerDetail(GeneralLedgerDetailDto generalLedgerDetail)
    {
        try
        {
            if (generalLedgerDetail == null)
            {
                return BadRequest("GeneralLedgerDetailDto cannot be null");
            }

            var result = await _generalLedgerDetailRepository.Update(generalLedgerDetail);

            if (!result)
            {
                _loggerService.SendError("An error occurred while updating the GeneralLedgerDetail");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDetail)}");
                return Ok(false);
            }
            else
            {
                _loggerService.SendDebugInfo($"GeneralLedgerDetail {generalLedgerDetail.Id} updated: {result}");
                return Ok(true);
            }
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in UpdateGeneralLedgerDetail", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("DeleteGeneralLedgerDetail/{generalLedgerDetailId}")]
    public async Task<ActionResult<bool>> DeleteGeneralLedgerDetail(int generalLedgerDetailId)
    {
        try
        {
            if (generalLedgerDetailId < 1)
            {
                return BadRequest("GeneralLedgerId cannot be 0");
            }

            var result = await _generalLedgerDetailRepository.Delete(generalLedgerDetailId);

            if (!result)
            {
                _loggerService.SendError($"An error occurred while deleting the GeneralLedgerDetail Id: {generalLedgerDetailId}");
                return Ok(false);
            }
            else
            {
                _loggerService.SendDebugInfo($"GeneralLedgerDetail {generalLedgerDetailId} deleted: {result}");
                return Ok(true);
            }
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in DeleteGeneralLedgerDetail", e);
            return new BadRequestResult();
        }
    }
}
