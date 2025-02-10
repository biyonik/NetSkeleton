using System.Security.Claims;

namespace Application.Authorization.Services;

/// <summary>
/// Mevcut kullanıcı bilgilerine erişim için servis interface'i
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Aktif kullanıcının ID'si
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// Aktif kullanıcının adı
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Aktif kullanıcının email adresi
    /// </summary>
    string Email { get; }

    /// <summary>
    /// Aktif kullanıcının rolleri
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Aktif kullanıcının claims'leri
    /// </summary>
    ClaimsPrincipal User { get; }

    /// <summary>
    /// Kullanıcının belirli bir role sahip olup olmadığını kontrol eder
    /// </summary>
    bool IsInRole(string role);

    /// <summary>
    /// Kullanıcının belirli bir yetkiye sahip olup olmadığını kontrol eder
    /// </summary>
    bool HasPermission(string permission);
}