using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Security.Response;
using Application.Common.Security.Settings;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityTokenException = Application.Common.Security.Exceptions.SecurityTokenException;

namespace Application.Common.Security.Services;

/// <summary>
/// JWT token servisi implementasyonu
/// </summary>
public class JwtService : IJwtService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        ILogger<JwtService> logger)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı için token üretir
    /// </summary>
    public async Task<TokenResponse> GenerateTokenAsync(ApplicationUser user)
    {
        try
        {
            // Claims oluştur
            var claims = await GetClaimsAsync(user);
            
            // Access token üret
            var accessToken = GenerateAccessToken(claims);
            
            // Refresh token üret
            var refreshToken = GenerateRefreshToken();
            
            // Kullanıcıya refresh token'ı kaydet
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            await _userManager.UpdateAsync(user);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user {UserId}", user.Id);
            throw;
        }
    }

    /// <summary>
    /// Refresh token ile yeni token üretir
    /// </summary>
    public async Task<TokenResponse> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                throw new SecurityTokenException("Invalid access token");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new SecurityTokenException("Invalid refresh token");

            return await GenerateTokenAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }

    /// <summary>
    /// Kullanıcının refresh token'ını iptal eder
    /// </summary>
    public async Task RevokeTokenAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Süresi geçmiş token'dan principal bilgisini çıkarır
    /// </summary>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Süresi geçmiş token'ı kabul et
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    /// <summary>
    /// Access token üretir
    /// </summary>
    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Refresh token üretir
    /// </summary>
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Kullanıcı için claims listesi oluşturur
    /// </summary>
    private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new("fullName", user.FullName)
        };

        // Rolleri ekle
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Permission'ları ekle
        var permissions = await _userManager.GetClaimsAsync(user);
        claims.AddRange(permissions.Where(x => x.Type == "Permission"));

        return claims;
    }
}