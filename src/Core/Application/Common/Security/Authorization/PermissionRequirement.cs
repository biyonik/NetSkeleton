using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Authorization;

/// <summary>
/// Permission bazlı yetkilendirme için requirement'lar
/// </summary>
public class PermissionRequirement(params string[] permissionSystemNames) : IAuthorizationRequirement
{
    /// <summary>
    /// Gerekli permission'ların system name'leri
    /// </summary>
    public string[] PermissionSystemNames { get; } = permissionSystemNames;
}