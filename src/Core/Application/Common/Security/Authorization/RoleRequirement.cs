using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Authorization;

/// <summary>
/// Rol bazlı yetkilendirme için requirement
/// </summary>
public class RoleRequirement(string[] roles) : IAuthorizationRequirement
{
    /// <summary>
    /// Gerekli roller
    /// </summary>
    public string[] Roles { get; } = roles;
}