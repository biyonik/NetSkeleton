using Domain.Authorization;

namespace Application.Authorization.Services;

public interface IAuthorizationService
{
    #region Permission Operations
    Task<Permission> CreatePermissionAsync(string name, string systemName, string? description = null, string? category = null, bool isSystemPermission = false, CancellationToken cancellationToken = default);
    Task UpdatePermissionAsync(Guid id, string name, string? description, string? category, CancellationToken cancellationToken = default);
    Task DeletePermissionAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetRequiredClaimsAsync(Guid id, string claims, CancellationToken cancellationToken = default);
    #endregion

    #region Permission Grant Operations
    Task<PermissionGrant> CreateGrantAsync(string userId, Guid permissionId, string? restrictions = null, string? validFrom = null, string? validTo = null, CancellationToken cancellationToken = default);
    Task UpdateGrantAsync(Guid id, string? restrictions, string? validFrom, string? validTo, CancellationToken cancellationToken = default);
    Task DeactivateGrantAsync(Guid id, CancellationToken cancellationToken = default);
    #endregion

    #region Authorization Operations
    Task<bool> ValidateAccessAsync(string userId, string permissionSystemName, string? context = null, CancellationToken cancellationToken = default);
    Task<IList<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> CheckResourceAccessAsync(string userId, string resource, string operation, CancellationToken cancellationToken = default);
    #endregion
}