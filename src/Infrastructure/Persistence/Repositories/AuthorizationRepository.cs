using Domain.Authorization;
using Domain.Authorization.Repositories;
using Domain.Authorization.Specifications;
using Domain.Common.Specifications;
using Infrastructure.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Context;

namespace Persistence.Repositories;

public class AuthorizationRepository(
    ApplicationDbContext context,
    ICacheManager cacheManager,
    ILogger<AuthorizationRepository> logger)
    : IAuthorizationRepository
{
    #region Permission Operations
    public async Task<Permission?> GetPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .Include(p => p.Grants)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Permission?> GetPermissionBySystemNameAsync(string systemName, CancellationToken cancellationToken = default)
    {
        var spec = new PermissionSpecifications.BySystemNameSpec(systemName);
        return await context.Permissions
            .WithSpecification(spec)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetAllPermissionsAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Permission> query = context.Permissions;
        
        if (!includeInactive)
        {
            var spec = new PermissionSpecifications.ActiveOnlySpec();
            query = query.WithSpecification(spec);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetPermissionsByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var spec = new PermissionSpecifications.ByCategorySpec(category);
        return await context.Permissions
            .WithSpecification(spec)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PermissionExistsAsync(string systemName, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .AnyAsync(p => p.SystemName == systemName && 
                          (excludeId == null || p.Id != excludeId), 
                cancellationToken);
    }

    public async Task<Permission> AddPermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await context.Permissions.AddAsync(permission, cancellationToken);
        return permission;
    }

    public async Task UpdatePermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        context.Permissions.Update(permission);
        await InvalidatePermissionCacheAsync(permission.Id, cancellationToken);
    }

    public async Task DeletePermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        context.Permissions.Remove(permission);
        await InvalidatePermissionCacheAsync(permission.Id, cancellationToken);
    }
    
    public async Task<List<Permission>> GetPermissionsWithSpecification(ISpecification<Permission> specification, CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .WithSpecification(specification)
            .ToListAsync(cancellationToken);
    }
    #endregion

    #region Permission Grant Operations
    public async Task<PermissionGrant?> GetGrantByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.PermissionGrants
            .Include(g => g.Permission)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<List<PermissionGrant>> GetUserGrantsAsync(string userId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var spec = new PermissionGrantSpecifications.ByUserIdSpec(userId, includeInactive);
        return await context.PermissionGrants
            .WithSpecification(spec)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PermissionGrant>> GetPermissionGrantsAsync(Guid permissionId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var spec = new PermissionGrantSpecifications.ByPermissionIdSpec(permissionId, includeInactive);
        return await context.PermissionGrants
            .WithSpecification(spec)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPermissionAsync(string userId, string permissionSystemName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user_has_permission:{userId}:{permissionSystemName}";
        var hasPermission = await cacheManager.GetAsync<bool?>(cacheKey, cancellationToken);

        if (hasPermission.HasValue)
            return hasPermission.Value;

        var spec = new PermissionGrantSpecifications.ValidGrantsSpec(userId, permissionSystemName);
        var exists = await context.PermissionGrants
            .WithSpecification(spec)
            .AnyAsync(cancellationToken);

        await cacheManager.SetAsync(cacheKey, exists, TimeSpan.FromMinutes(30), cancellationToken);
        return exists;
    }

    public async Task<bool> HasAnyPermissionAsync(string userId, IEnumerable<string> permissionSystemNames, CancellationToken cancellationToken = default)
    {
        foreach (var permissionName in permissionSystemNames)
        {
            if (await HasPermissionAsync(userId, permissionName, cancellationToken))
                return true;
        }
        return false;
    }

    public async Task<List<string>> GetUserPermissionSystemNamesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user_permission_names:{userId}";
        var permissionNames = await cacheManager.GetAsync<List<string>>(cacheKey, cancellationToken);

        if (permissionNames != null)
            return permissionNames;

        var spec = new PermissionGrantSpecifications.ByUserIdSpec(userId);
        var grants = await context.PermissionGrants
            .WithSpecification(spec)
            .Select(g => g.Permission.SystemName)
            .ToListAsync(cancellationToken);

        await cacheManager.SetAsync(cacheKey, grants, TimeSpan.FromMinutes(30), cancellationToken);
        return grants;
    }

    public async Task<PermissionGrant> AddGrantAsync(PermissionGrant grant, CancellationToken cancellationToken = default)
    {
        await context.PermissionGrants.AddAsync(grant, cancellationToken);
        await InvalidateUserPermissionCacheAsync(grant.UserId, cancellationToken);
        return grant;
    }

    public async Task UpdateGrantAsync(PermissionGrant grant, CancellationToken cancellationToken = default)
    {
        context.PermissionGrants.Update(grant);
        await InvalidateUserPermissionCacheAsync(grant.UserId, cancellationToken);
    }

    public async Task DeleteGrantAsync(PermissionGrant grant, CancellationToken cancellationToken = default)
    {
        context.PermissionGrants.Remove(grant);
        await InvalidateUserPermissionCacheAsync(grant.UserId, cancellationToken);
    }
    #endregion
    
    #region Permission Endpoint Operations
    public async Task<List<Permission>> GetPermissionsForEndpointAsync(
        string controller,
        string action,
        string httpMethod,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"endpoint_permissions:{controller}:{action}:{httpMethod}";
        var permissions = await cacheManager.GetAsync<List<Permission>>(cacheKey, cancellationToken);

        if (permissions == null)
        {
            permissions = await context.Permissions
                .Include(p => p.Endpoints)
                .Where(p => p.IsActive && p.Endpoints.Any(e =>
                    e.Controller == controller &&
                    e.Action == action &&
                    e.HttpMethod == httpMethod))
                .ToListAsync(cancellationToken);

            await cacheManager.SetAsync(cacheKey, permissions, TimeSpan.FromMinutes(30), cancellationToken);
        }

        return permissions;
    }

    public async Task<List<PermissionEndpoint>> GetEndpointsForPermissionAsync(
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        return await context.PermissionEndpoints
            .Where(e => e.PermissionId == permissionId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddEndpointToPermissionAsync(
        PermissionEndpoint endpoint,
        CancellationToken cancellationToken = default)
    {
        await context.PermissionEndpoints.AddAsync(endpoint, cancellationToken);
        await InvalidateEndpointPermissionCacheAsync(
            endpoint.Controller, 
            endpoint.Action, 
            endpoint.HttpMethod, 
            cancellationToken);
    }

    public async Task RemoveEndpointFromPermissionAsync(
        Guid endpointId,
        CancellationToken cancellationToken = default)
    {
        var endpoint = await context.PermissionEndpoints
            .FirstOrDefaultAsync(e => e.Id == endpointId, cancellationToken);

        if (endpoint != null)
        {
            context.PermissionEndpoints.Remove(endpoint);
            await InvalidateEndpointPermissionCacheAsync(
                endpoint.Controller,
                endpoint.Action,
                endpoint.HttpMethod,
                cancellationToken);
        }
    }

    private async Task InvalidateEndpointPermissionCacheAsync(
        string controller,
        string action,
        string httpMethod,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"endpoint_permissions:{controller}:{action}:{httpMethod}";
        await cacheManager.RemoveAsync(cacheKey, cancellationToken);
    }
    #endregion

    #region Authorization Check Operations
    public async Task<bool> IsValidGrantAsync(Guid grantId, CancellationToken cancellationToken = default)
    {
        var grant = await GetGrantByIdAsync(grantId, cancellationToken);
        if (grant == null || !grant.IsActive || !grant.Permission.IsActive)
            return false;

        // Geçerlilik tarihi kontrolü
        if (!string.IsNullOrEmpty(grant.ValidTo) && 
            DateTime.Parse(grant.ValidTo) < DateTime.UtcNow)
            return false;

        if (!string.IsNullOrEmpty(grant.ValidFrom) && 
            DateTime.Parse(grant.ValidFrom) > DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<bool> CanAccessResourceAsync(string userId, string resource, string operation, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user_resource_access:{userId}:{resource}:{operation}";
        var canAccess = await cacheManager.GetAsync<bool?>(cacheKey, cancellationToken);

        if (canAccess.HasValue)
            return canAccess.Value;

        // Resource ve operation'a göre gerekli permission'ları belirle
        var requiredPermission = $"{resource}.{operation}";
        var result = await HasPermissionAsync(userId, requiredPermission, cancellationToken);

        await cacheManager.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30), cancellationToken);
        return result;
    }

    public async Task<(bool HasAccess, string? Reason)> EvaluateAccessAsync(
        string userId, 
        string permissionSystemName, 
        string? context1 = null, 
        CancellationToken cancellationToken = default)
    {
        var spec = new PermissionGrantSpecifications.ValidGrantsSpec(userId, permissionSystemName);
        var grant = await context.PermissionGrants
            .WithSpecification(spec)
            .FirstOrDefaultAsync(cancellationToken);

        if (grant == null)
            return (false, "No valid grant found");

        if (!string.IsNullOrEmpty(grant.Restrictions))
        {
            // TODO: Restriction değerlendirmesi yapılacak
            // Örnek: JSON olarak saklanan restriction'ları parse edip
            // context ile karşılaştırma yapılabilir
            logger.LogInformation("Grant restrictions found but not evaluated: {restrictions}", grant.Restrictions);
        }

        return (true, null);
    }
    #endregion

    #region Cache Operations
    public async Task InvalidateUserPermissionCacheAsync(string userId, CancellationToken cancellationToken = default)
    {
        await cacheManager.RemoveAsync($"user_permission_names:{userId}", cancellationToken);
        
        // Permission bazlı cache'leri temizle
        var permissions = await GetUserPermissionSystemNamesAsync(userId, cancellationToken);
        foreach (var permission in permissions)
        {
            await cacheManager.RemoveAsync($"user_has_permission:{userId}:{permission}", cancellationToken);
        }
    }

    public async Task InvalidatePermissionCacheAsync(Guid permissionId, CancellationToken cancellationToken = default)
    {
        var permission = await GetPermissionByIdAsync(permissionId, cancellationToken);
        if (permission == null) return;

        // Bu permission'a sahip tüm kullanıcıların cache'ini temizle
        var grants = await GetPermissionGrantsAsync(permissionId, true, cancellationToken);
        foreach (var grant in grants)
        {
            await InvalidateUserPermissionCacheAsync(grant.UserId, cancellationToken);
        }
    }

    public async Task InvalidateAllPermissionCachesAsync(CancellationToken cancellationToken = default)
    {
        // Tüm permission pattern'lerine sahip key'leri temizle
        await cacheManager.RemoveByPatternAsync("user_permission_names:*", cancellationToken);
        await cacheManager.RemoveByPatternAsync("user_has_permission:*", cancellationToken);
        await cacheManager.RemoveByPatternAsync("user_resource_access:*", cancellationToken);
    }
    #endregion
}