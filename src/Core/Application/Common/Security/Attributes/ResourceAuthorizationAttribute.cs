using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Attributes;

/// <summary>
/// Resource bazlı authorization attribute'u
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ResourceAuthorizationAttribute(string resource, string operation)
    : AuthorizeAttribute($"Resource_{resource}_{operation}");