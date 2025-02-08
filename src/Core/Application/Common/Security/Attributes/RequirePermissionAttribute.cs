using Domain.Security;
using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Attributes;

/// <summary>
/// Permission enumeration'ı ile authorization attribute'u
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(params Permission[] permissions)
        : base($"Permission_{string.Join(",", permissions.Select(p => p.Code))}")
    {
    }
}