using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Authorization;

/// <summary>
/// Resource bazlı yetkilendirme için requirement
/// </summary>
public class ResourceOperationRequirement(string resource, string operation, object? context = null)
    : IAuthorizationRequirement
{
    /// <summary>
    /// Resource adı
    /// </summary>
    public string Resource { get; } = resource;

    /// <summary>
    /// Operation adı
    /// </summary>
    public string Operation { get; } = operation;

    /// <summary>
    /// Context data (opsiyonel)
    /// </summary>
    public object? Context { get; } = context;
}