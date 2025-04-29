using CommunityToolkit.Mvvm.Messaging;
using dotNIES.API.Controllers;
using dotNIES.API.Core.Services;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.common.Controllers;
[Area("common")]
[Route("api/[area]/[controller]")]
[ApiController]
public class CategoryController : BaseController
{
    private readonly IBaseDataService _baseDataService;

    public CategoryController(ILoggerService loggerService,
                              IAppInfoDto appInfoDto,
                              IUserAppInfoDto userAppInfoDto, IBaseDataService baseDataService) : base(loggerService, appInfoDto, userAppInfoDto)
    {
        _baseDataService = baseDataService;
    }

    [HttpGet]
    public IActionResult<IEnumerable<CategoryDto>> GetAll()
    {
        try
        {
            var result
        }
        catch (Exception e)
        {

            throw;
        }
    }
}
