using Domain.Common.Specifications;

namespace Domain.Authorization.Repositories;

/// <summary>
/// Authorization işlemleri için repository interface
/// </summary>
public interface IAuthorizationRepository
{
    #region Permission Operations
    Task<Permission?> GetPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Permission?> GetPermissionBySystemNameAsync(string systemName, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetAllPermissionsAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetPermissionsByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<bool> PermissionExistsAsync(string systemName, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Permission> AddPermissionAsync(Permission permission, CancellationToken cancellationToken = default);
    Task UpdatePermissionAsync(Permission permission, CancellationToken cancellationToken = default);
    Task DeletePermissionAsync(Permission permission, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetPermissionsWithSpecification(ISpecification<Permission> specification, CancellationToken cancellationToken = default);
    #endregion

    #region Permission Grant Operations
    Task<PermissionGrant?> GetGrantByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PermissionGrant>> GetUserGrantsAsync(string userId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<List<PermissionGrant>> GetPermissionGrantsAsync(Guid permissionId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(string userId, string permissionSystemName, CancellationToken cancellationToken = default);
    Task<bool> HasAnyPermissionAsync(string userId, IEnumerable<string> permissionSystemNames, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserPermissionSystemNamesAsync(string userId, CancellationToken cancellationToken = default);
    Task<PermissionGrant> AddGrantAsync(PermissionGrant grant, CancellationToken cancellationToken = default);
    Task UpdateGrantAsync(PermissionGrant grant, CancellationToken cancellationToken = default);
    Task DeleteGrantAsync(PermissionGrant grant, CancellationToken cancellationToken = default);
    #endregion
    
    #region Permission Endpoint Operations
    Task<List<Permission>> GetPermissionsForEndpointAsync(
        string controller, 
        string action, 
        string httpMethod,
        CancellationToken cancellationToken = default);

    Task<List<PermissionEndpoint>> GetEndpointsForPermissionAsync(
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task AddEndpointToPermissionAsync(
        PermissionEndpoint endpoint,
        CancellationToken cancellationToken = default);

    Task RemoveEndpointFromPermissionAsync(
        Guid endpointId,
        CancellationToken cancellationToken = default);
    #endregion

    #region Authorization Check Operations
    Task<bool> IsValidGrantAsync(Guid grantId, CancellationToken cancellationToken = default);
    Task<bool> CanAccessResourceAsync(string userId, string resource, string operation, CancellationToken cancellationToken = default);
    Task<(bool HasAccess, string? Reason)> EvaluateAccessAsync(string userId, string permissionSystemName, string? context = null, CancellationToken cancellationToken = default);
    #endregion

    #region Cache Operations
    Task InvalidateUserPermissionCacheAsync(string userId, CancellationToken cancellationToken = default);
    Task InvalidatePermissionCacheAsync(Guid permissionId, CancellationToken cancellationToken = default);
    Task InvalidateAllPermissionCachesAsync(CancellationToken cancellationToken = default);
    #endregion
}