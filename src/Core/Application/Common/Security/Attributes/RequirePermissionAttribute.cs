using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Attributes;

/// <summary>
/// Permission bazlı yetkilendirme için attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionSystemName) 
        : base($"Permission_{permissionSystemName}")
    {
    }

    public RequirePermissionAttribute(params string[] permissionSystemNames)
        : base($"Permission_{string.Join(",", permissionSystemNames)}")
    {
    }
}