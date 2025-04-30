using dotNIES.API.Controllers;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace dotNIES.API.Areas.fin.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GeneralLedgerDetailController(ILoggerService loggerService,
                                           IAppInfoDto appInfoDto,
                                           IUserAppInfoDto userAppInfoDto) : BaseController(loggerService, appInfoDto, userAppInfoDto)
{
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IUserAppInfoDto _userAppInfoDto = userAppInfoDto;

    [HttpGet("GetGeneralLedgerDetails")]
    public async Task<ActionResult<IEnumerable<GeneralLedgerDetailDto>>> GetGeneralLedgerDetails([FromBody] int generalLedgerId)
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
            _loggerService.SendError("An exception occurred while getting all records from GetGeneralLedgerDetails", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("InsertGeneralLedgerDetail")]
    public async Task<ActionResult<int>> InsertGeneralLedgerDetail([FromBody] GeneralLedgerDetailDto generalLedgerDetail)
    {
        try
        {
            if (generalLedgerDetail == null)
            {
                return BadRequest("GeneralLedgerDetailDto cannot be null");
            }

            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in InsertGeneralLedgerDetail", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("UpdateGeneralLedgerDetail")]
    public async Task<ActionResult<bool>> UpdateGeneralLedgerDetail([FromBody] GeneralLedgerDetailDto generalLedgerDetail)
    {
        try
        {
            if (generalLedgerDetail == null)
            {
                return BadRequest("GeneralLedgerDetailDto cannot be null");
            }

            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in UpdateGeneralLedgerDetail", e);
            return new BadRequestResult();
        }
    }

    [HttpPost("DeleteGeneralLedgerDetail")]
    public async Task<ActionResult<bool>> DeleteGeneralLedgerDetail([FromBody] int generalLedgerDetailId)
    {
        try
        {
            if (generalLedgerDetailId < 1)
            {
                return BadRequest("GeneralLedgerId cannot be 0");
            }

            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred in DeleteGeneralLedgerDetail", e);
            return new BadRequestResult();
        }
    }
}
