using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _loggerService;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(IConfiguration configuration,
                          ILoggerService loggerService,
                          IAppInfoDto appInfoDto,
                          IUserAppInfoDto userAppInfoDto,
                          UserManager<IdentityUser> userManager
                         ) : base(loggerService, appInfoDto, userAppInfoDto)
    {
        _configuration = configuration;
        _loggerService = loggerService;
        _userManager = userManager;
    }

    [HttpPost("login/{login}")]
    public async Task<IActionResult> Login(LoginModel login)
    {
        var isValid = await IsValidUser(login);

        if (isValid)
        {
            var token = GenerateJwtToken(login.Username);

            _loggerService.SendInformation($"Login attempt SUCCEEDED for username {login.Username}");

            return Ok(new { token });
        }

        _loggerService.SendInformation($"Login attempt FAILED for username {login.Username}");

        return Unauthorized();
    }

    private async Task<bool> IsValidUser(LoginModel login)
    {
        // TODO: db validatie
        //return login.Username == "gebruiker" && login.Password == "wachtwoord";

        // UserManager is de standaard manier om gebruikers te beheren in ASP.NET Identity
        var user = await _userManager.FindByNameAsync(login.Username);

        if (user == null)
        {
            return false; // Gebruiker niet gevonden
        }

        // Controleer of het wachtwoord correct is
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, login.Password);

        if (!isPasswordValid)
        {
            return false; // Onjuist wachtwoord
        }

        // Controleer of de gebruiker is vergrendeld
        if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
        {
            return false; // Gebruiker is vergrendeld
        }

        // TODO: require e-mail validatie
        //// Controleer of e-mail is bevestigd (optioneel)
        //if (requireEmailConfirmation && !user.EmailConfirmed)
        //{
        //    return false; // E-mail is niet bevestigd
        //}

        // Extra validaties kunnen hier worden toegevoegd
        // - Controle op specifieke rollen
        // - Controle op specifieke claims
        // - Controle op twee-factor authenticatie, etc.

        return true; // Alle validaties zijn geslaagd
    }

    private string GenerateJwtToken(string username)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),            
            new Claim(ClaimTypes.Role, "User")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
