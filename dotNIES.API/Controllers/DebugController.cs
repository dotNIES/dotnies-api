using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace dotNIES.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DebugController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DebugController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet("auth-debug")]
    public IActionResult AuthDebug()
    {
        // Juiste token extractie
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader?.Split(" ").Length > 1 ? authHeader.Split(" ")[1] : null;

        Console.WriteLine($"Full Auth Header: {authHeader}");
        Console.WriteLine($"Extracted Token: {token}");

        if (token == null)
        {
            return BadRequest("No token provided or incorrect format. Use 'Bearer {token}'");
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            return Ok(new
            {
                FullAuthHeader = authHeader,
                ExtractedToken = token,
                AuthenticationType = principal.Identity.AuthenticationType,
                IsAuthenticated = principal.Identity.IsAuthenticated,
                Name = principal.Identity.Name,
                Claims = principal.Claims.Select(c => new { c.Type, c.Value })
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation error: {ex}");
            return BadRequest($"Token validation failed: {ex.Message}");
        }
    }

    [AllowAnonymous]
    [HttpGet("validate-token")]
    public IActionResult ValidateToken()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("No token provided");
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Ok("Token is valid");
        }
        catch (Exception ex)
        {
            return Unauthorized($"Token validation failed: {ex.Message}");
        }
    }
}
