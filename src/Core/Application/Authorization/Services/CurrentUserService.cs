using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Authorization.Services;

/// <summary>
/// Mevcut kullanıcı bilgilerine HTTP context üzerinden erişim sağlayan servis
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Aktif kullanıcının ID'si
    /// </summary>
    public string UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

    /// <summary>
    /// Aktif kullanıcının adı
    /// </summary>
    public string UserName => User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";

    /// <summary>
    /// Aktif kullanıcının email adresi
    /// </summary>
    public string Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? "system@domain.com";

    /// <summary>
    /// Aktif kullanıcının rolleri
    /// </summary>
    public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role).Select(x => x.Value) ?? Array.Empty<string>();

    /// <summary>
    /// Kullanıcının belirli bir role sahip olup olmadığını kontrol eder
    /// </summary>
    public bool IsInRole(string role)
    {
        return User?.IsInRole(role) ?? false;
    }

    /// <summary>
    /// Kullanıcının belirli bir yetkiye sahip olup olmadığını kontrol eder
    /// Custom claim'ler üzerinden yetki kontrolü yapar
    /// </summary>
    public bool HasPermission(string permission)
    {
        var permissions = User?.FindAll("Permission").Select(x => x.Value) ?? Array.Empty<string>();
        return permissions.Contains(permission);
    }
}