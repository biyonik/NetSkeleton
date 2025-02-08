using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Attributes;

/// <summary>
/// Permission bazlı authorization attribute'u
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) 
        : base($"Permission_{permission}")
    {
    }

    public HasPermissionAttribute(params string[] permissions)
        : base($"Permission_{string.Join(",", permissions)}")
    {
    }
}