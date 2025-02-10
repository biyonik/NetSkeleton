using Domain.Authorization;
using Domain.Authorization.Repositories;
using Domain.Authorization.Rules;
using Domain.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Authorization.Services;

public class AuthorizationService(
    IAuthorizationRepository repository,
    ILogger<AuthorizationService> logger)
    : IAuthorizationService
{
    #region Permission Operations
    public async Task<Permission> CreatePermissionAsync(
        string name,
        string systemName,
        string? description = null,
        string? category = null,
        bool isSystemPermission = false,
        CancellationToken cancellationToken = default)
    {
        // Business Rules
        if (!await new PermissionSystemNameMustBeUniqueRule(repository, systemName)
            .IsValid())
        {
            throw new BusinessRuleValidationException("PermissionSystemNameMustBeUnique","Permission system name must be unique");
        }

        var permission = Permission.Create(
            name,
            systemName,
            description,
            category,
            isSystemPermission);

        return await repository.AddPermissionAsync(permission, cancellationToken);
    }

    public async Task UpdatePermissionAsync(
        Guid id,
        string name,
        string? description,
        string? category,
        CancellationToken cancellationToken = default)
    {
        var permission = await repository.GetPermissionByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Permission with ID {id} not found.");

        // Business Rules
        if (!await new SystemPermissionCannotBeModifiedRule(permission).IsValid())
        {
            throw new BusinessRuleValidationException("SystemPermissionCannotBeModified","System permissions cannot be modified");
        }

        if (!await new PermissionSystemNameMustBeUniqueRule(repository, permission.SystemName, id)
            .IsValid())
        {
            throw new BusinessRuleValidationException("SystemPermissionCannotBeModified","Permission system name must be unique");
        }

        permission.Update(name, description, category);
        await repository.UpdatePermissionAsync(permission, cancellationToken);
    }

    public async Task DeletePermissionAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var permission = await repository.GetPermissionByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Permission with ID {id} not found.");

        // Business Rules
        if (!await new SystemPermissionCannotBeModifiedRule(permission).IsValid())
        {
            throw new BusinessRuleValidationException("SystemPermissionCannotBeModified","System permissions cannot be deleted");
        }

        await repository.DeletePermissionAsync(permission, cancellationToken);
    }

    public async Task SetRequiredClaimsAsync(
        Guid id,
        string claims,
        CancellationToken cancellationToken = default)
    {
        var permission = await repository.GetPermissionByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Permission with ID {id} not found.");

        // Business Rules
        if (!await new SystemPermissionCannotBeModifiedRule(permission).IsValid())
        {
            throw new BusinessRuleValidationException("SystemPermissionCannotBeModified","System permissions cannot be modified");
        }

        permission.SetRequiredClaims(claims);
        await repository.UpdatePermissionAsync(permission, cancellationToken);
    }
    #endregion

    #region Permission Grant Operations
    public async Task<PermissionGrant> CreateGrantAsync(
        string userId,
        Guid permissionId,
        string? restrictions = null,
        string? validFrom = null,
        string? validTo = null,
        CancellationToken cancellationToken = default)
    {
        var permission = await repository.GetPermissionByIdAsync(permissionId, cancellationToken)
            ?? throw new NotFoundException($"Permission with ID {permissionId} not found.");

        // Business Rules
        if (!await new PermissionMustBeActiveRule(permission).IsValid())
        {
            throw new BusinessRuleValidationException("PermissionMustBeActive","Cannot grant inactive permission");
        }

        if (!await new GrantDatesMustBeValidRule(validFrom, validTo).IsValid())
        {
            throw new BusinessRuleValidationException("GrantDatesMustBeValid","Invalid grant dates");
        }

        if (!await new DuplicateActiveGrantNotAllowedRule(repository, userId, permissionId).IsValid())
        {
            throw new BusinessRuleValidationException("DuplicateActiveGrantNotAllowed","User already has an active grant for this permission");
        }

        if (!await new RequiredClaimsMustBePresentRule(repository, userId, permission).IsValid())
        {
            throw new BusinessRuleValidationException("RequiredClaimsMustBePresent","User does not have required claims");
        }

        if (restrictions != null && !await new AccessRulesFormatRule(restrictions).IsValid())
        {
            throw new BusinessRuleValidationException("InvalidAccessRulesFormat","Invalid access rules format");
        }

        var grant = PermissionGrant.Create(
            userId,
            permissionId,
            restrictions,
            validFrom,
            validTo);

        return await repository.AddGrantAsync(grant, cancellationToken);
    }

    public async Task UpdateGrantAsync(
        Guid id,
        string? restrictions,
        string? validFrom,
        string? validTo,
        CancellationToken cancellationToken = default)
    {
        var grant = await repository.GetGrantByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Permission grant with ID {id} not found.");

        // Business Rules
        if (!await new GrantDatesMustBeValidRule(validFrom, validTo).IsValid())
        {
            throw new BusinessRuleValidationException("GrantDatesMustBeValid", "Invalid grant dates");
        }

        if (!await new DuplicateActiveGrantNotAllowedRule(repository, grant.UserId, grant.PermissionId, id).IsValid())
        {
            throw new BusinessRuleValidationException("DuplicateActiveGrantNotAllowed","User already has an active grant for this permission");
        }

        if (restrictions != null && !await new AccessRulesFormatRule(restrictions).IsValid())
        {
            throw new BusinessRuleValidationException("InvalidAccessRulesFormat","Invalid access rules format");
        }

        grant.Update(restrictions, validFrom, validTo);
        await repository.UpdateGrantAsync(grant, cancellationToken);
    }

    public async Task DeactivateGrantAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var grant = await repository.GetGrantByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Permission grant with ID {id} not found.");

        grant.Deactivate();
        await repository.UpdateGrantAsync(grant, cancellationToken);
    }
    #endregion

    #region Authorization Operations
    public async Task<bool> ValidateAccessAsync(
        string userId,
        string permissionSystemName,
        string? context = null,
        CancellationToken cancellationToken = default)
    {
        var (hasAccess, _) = await repository.EvaluateAccessAsync(
            userId,
            permissionSystemName,
            context,
            cancellationToken);

        return hasAccess;
    }

    public async Task<IList<string>> GetUserPermissionsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetUserPermissionSystemNamesAsync(userId, cancellationToken);
    }

    public async Task<bool> CheckResourceAccessAsync(
        string userId,
        string resource,
        string operation,
        CancellationToken cancellationToken = default)
    {
        return await repository.CanAccessResourceAsync(userId, resource, operation, cancellationToken);
    }
    #endregion
}