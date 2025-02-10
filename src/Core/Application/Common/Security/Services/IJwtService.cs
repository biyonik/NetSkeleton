using System.Security.Claims;
using Application.Common.Security.Response;
using Domain.Identity.Models;

namespace Application.Common.Security.Services;

/// <summary>
/// JWT token servisi
/// </summary>
public interface IJwtService
{
    Task<TokenResponse> GenerateTokenAsync(ApplicationUser user);
    Task<TokenResponse> RefreshTokenAsync(string accessToken, string refreshToken);
    Task RevokeTokenAsync(string userId);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}