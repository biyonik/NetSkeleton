using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Authorization;

/// <summary>
/// Permission bazlı yetkilendirme için requirement
/// </summary>
public class PermissionRequirement(string[] permissions) : IAuthorizationRequirement
{
    /// <summary>
    /// Gerekli permission'lar
    /// </summary>
    public string[] Permissions { get; } = permissions;
}