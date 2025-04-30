using dotNIES.API.Controllers;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.fin.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GeneralLedgerController(ILoggerService loggerService,
                                     IAppInfoDto appInfoDto,
                                     IUserAppInfoDto userAppInfoDto) : BaseController(loggerService, appInfoDto, userAppInfoDto)
{
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IUserAppInfoDto _userAppInfoDto = userAppInfoDto;

    [HttpGet("GetGeneralLedgers")]
    public async Task<ActionResult<IEnumerable<GeneralLedgerDto>>> GetGeneralLedgers()
    {
        try
        {
            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GetGeneralLedgers", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("InsertGeneralLedger")]
    public async Task<ActionResult<int>> InsertGeneralLedger([FromBody] GeneralLedgerDto generalLedger)
    {
        try
        {
            if (generalLedger == null)
            {
                return BadRequest("GeneralLedgerDto cannot be null");
            }

            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in InsertGeneralLedger", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("UpdategeneralLedger")]
    public async Task<ActionResult<bool>> UpdategeneralLedger([FromBody] GeneralLedgerDto generalLedger)
    {
        try
        {
            if (generalLedger == null)
            {
                return BadRequest("GeneralLedgerDto cannot be null");
            }

            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in UpdateGeneralLedger", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("DeletegeneralLedger")]
    public async Task<ActionResult<bool>> DeletegeneralLedger([FromBody] int generalLedgerId)
    {
        try
        {
            if (generalLedgerId < 1)
            {
                return BadRequest("GeneralLedgerId cannot be 0");
            }

            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in DeleteGeneralLedger", e);
            return new BadRequestResult();
        }
    }
}
