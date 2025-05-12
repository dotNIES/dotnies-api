using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Dto.Internal.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dotNIES.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _loggerService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IBaseRepository _baseRepository;

    public AuthController(IConfiguration configuration,
                          ILoggerService loggerService,
                          IAppInfoDto appInfoDto,
                          IUserAppInfoDto userAppInfoDto,
                          UserManager<IdentityUser> userManager,
                          IBaseRepository baseRepository
                         ) : base(loggerService, appInfoDto, userAppInfoDto)
    {
        _configuration = configuration;
        _loggerService = loggerService;
        _userManager = userManager;
        _baseRepository = baseRepository;
    }

    [HttpPost("login/{login}")]
    public async Task<IActionResult> Login(LoginModel login)
    {
        var isValid = await IsValidUser(login);

        if (isValid)
        {
            var token = GenerateJwtToken(login.Username);
            var refreshToken = GenerateRefreshToken(login.Username);

            _loggerService.SendInformation($"Login attempt SUCCEEDED for username {login.Username}");

            // save the refresh token in the database
            SaveRefreshToken(login.Username, refreshToken);

            return Ok(new { token, refreshToken });
        }

        _loggerService.SendInformation($"Login attempt FAILED for username {login.Username}");

        return Unauthorized();
    }

    [HttpPost("refresh-token/{request}")]
    public async Task<IActionResult> RefreshToken(RefreshTokenModel request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest("Refresh token is required");

        try
        {
            // Verify the refresh token
            var principal = GetPrincipalFromToken(request.RefreshToken, _configuration["Jwt:RefreshTokenKey"]);

            if (principal == null)
                return BadRequest("Invalid refresh token");

            var username = principal.Identity.Name;

            // Check if the token exists in the database and is valid
            var storedToken = await GetRefreshToken(username);

            if (storedToken == null)
                return BadRequest("Invalid or expired refresh token");

            // Generate a new access token
            var newAccessToken = GenerateJwtToken(username);

            return Ok(new { accessToken = newAccessToken });
        }
        catch (Exception ex)
        {
            _loggerService.SendError($"Error processing refresh token: {ex.Message}");
            return BadRequest("Invalid refresh token");
        }
    }

    private async Task SaveRefreshToken(string? username, string? refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(refreshToken));

            await _baseRepository.InsertAsync<UserTokenDto>(new UserTokenDto
            {
                Id = Guid.NewGuid(),
                IsRevoked = false,
                RefreshToken = refreshToken,
                User = username,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    Convert.ToDouble(_configuration["Jwt:RefreshTokenDurationInDays"]))
            });

            _loggerService.SendInformation($"Refresh token for {username} saved.");
        }
        catch (Exception e)
        {
            _loggerService.SendError("Saving refresh token threw exceptions", e);
            throw;
        }
    }

    private async Task<string?> GetRefreshToken(string? username)
    {
        try
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            var result = await _baseRepository.GetDataAsync<UserTokenDto>(
                        $"SELECT TOP(1) * FROM UserTokens WHERE User = '{username}' AND " +
                        $"ExpiresAt >= '{DateTime.Now}' ORDER BY ExpiresAt DESC");

            return result.FirstOrDefault()?.RefreshToken;
        }
        catch (Exception e)
        {
            _loggerService.SendError("Getting refresh token threw exceptions", e);
            return null;
        }
    }

    private async Task<bool> IsValidUser(LoginModel login)
    {
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

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Log de gegenereerde token (alleen in development!)
        _loggerService.SendInformation($"Generated token length: {tokenString.Length}");

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(string username)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            // Voeg eventueel andere claims toe die u nodig heeft
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:RefreshTokenKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(
                Convert.ToDouble(_configuration["Jwt:RefreshTokenDurationInDays"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
