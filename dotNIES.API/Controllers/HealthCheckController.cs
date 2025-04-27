using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("/")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    /// <summary>
    /// This is the default endpoint for the API. It returns a message indicating that the API is running.
    /// </summary>
    /// <remarks>
    /// Deployment to Railway needs this endpoint to be available, otherwise the deployment will fail.
    /// </remarks>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get() => Ok("This is the dotNIES API. If you do not know what this is, please close the browserscreen. Thank you and goodbeye!");

    /// <summary>
    /// Use this endpoint to check if the API is online (returning 1).
    /// </summary>
    /// <returns></returns>
    [HttpGet("IsOnline")]
    public IActionResult IsOnline() => Ok(1);
}
