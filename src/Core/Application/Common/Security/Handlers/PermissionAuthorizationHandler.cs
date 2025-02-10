using System.Security.Claims;
using Application.Common.Security.Authorization;
using Domain.Authorization.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Application.Common.Security.Handlers;

/// <summary>
/// Permission bazlı yetkilendirme handler'ı
/// </summary>
public class PermissionAuthorizationHandler(
    IAuthorizationRepository authorizationRepository,
    ILogger<PermissionAuthorizationHandler> logger)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        try
        {
            var user = context.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                logger.LogWarning("User is not authenticated");
                return;
            }

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID claim not found");
                return;
            }

            // Tüm permission'ları kontrol et
            var hasAllPermissions = true;
            foreach (var permissionName in requirement.PermissionSystemNames)
            {
                var hasPermission = await authorizationRepository.HasPermissionAsync(
                    userId, permissionName);

                if (!hasPermission)
                {
                    hasAllPermissions = false;
                    break;
                }
            }

            if (hasAllPermissions)
            {
                context.Succeed(requirement);
                logger.LogInformation(
                    "User {UserId} authorized with permissions: {Permissions}",
                    userId,
                    string.Join(", ", requirement.PermissionSystemNames));
            }
            else
            {
                logger.LogWarning(
                    "User {UserId} does not have required permissions: {Permissions}",
                    userId,
                    string.Join(", ", requirement.PermissionSystemNames));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in permission authorization");
        }
    }
}