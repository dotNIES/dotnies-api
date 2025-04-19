using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello from dotNIES API");
    }
}
