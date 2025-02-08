using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Authorization;

/// <summary>
/// Resource bazlı yetkilendirme için requirement
/// </summary>
public class ResourceOperationRequirement(string resource, string operation) : IAuthorizationRequirement
{
    /// <summary>
    /// Resource tipi
    /// </summary>
    public string Resource { get; } = resource;

    /// <summary>
    /// İşlem tipi
    /// </summary>
    public string Operation { get; } = operation;
}