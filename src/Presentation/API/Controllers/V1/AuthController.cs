

using Application.Auth.Commands;
using Application.Common.Results;
using Application.Common.Security.Attributes;
using Application.Common.Security.Response;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;

[ApiVersion("1.0")]
public class AuthController : BaseApiController
{
    /// <summary>
    /// Kullanıcı girişi yapar
    /// </summary>
    /// <param name="command">Email ve şifre bilgileri</param>
    /// <response code="200">Giriş başarılı - Token bilgileri döner</response>
    /// <response code="401">Giriş başarısız</response>
    /// <response code="400">Validasyon hatası</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    /// <summary>
    /// Kullanıcı kaydı oluşturur
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    /// <summary>
    /// Refresh token ile yeni token üretir
    /// </summary>
    /// <param name="command">Access token ve refresh token bilgileri</param>
    /// <response code="200">Yenileme başarılı - Yeni token bilgileri döner</response>
    /// <response code="401">Geçersiz veya süresi dolmuş token</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    /// <summary>
    /// Kullanıcının refresh token'ını iptal eder
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    /// <response code="200">Token iptali başarılı</response>
    /// <response code="403">Yetki hatası</response>
    [HttpPost("revoke-token/{userId}")]
    [RequirePermission("Auth.RevokeToken")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RevokeToken(string userId)
    {
        var result = await Mediator.Send(new RevokeTokenCommand(userId));
        return FromResult(result);
    }
}