using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Controllers;
[Route("/")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("This is the dotNIES API. If you do not know what this is, please close the browserscreen. Thank you and goodbeye!"); 
}
