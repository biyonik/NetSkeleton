using System.Security.Claims;
using Application.Common.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Application.Common.Security.Handlers;

/// <summary>
/// Permission bazlı yetkilendirme handler'ı
/// </summary>
public class PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        try
        {
            var user = context.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                logger.LogWarning("User is not authenticated");
                return Task.CompletedTask;
            }

            // Super admin her zaman erişebilir
            if (user.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Kullanıcının permission'larını kontrol et
            var userPermissions = user.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToArray();

            var hasAllPermissions = requirement.Permissions
                .All(permission => userPermissions.Contains(permission));

            if (hasAllPermissions)
            {
                context.Succeed(requirement);
                logger.LogInformation(
                    "User {UserId} authorized with permissions: {Permissions}",
                    user.FindFirstValue(ClaimTypes.NameIdentifier),
                    string.Join(", ", requirement.Permissions));
            }
            else
            {
                logger.LogWarning(
                    "User {UserId} does not have required permissions: {Permissions}",
                    user.FindFirstValue(ClaimTypes.NameIdentifier),
                    string.Join(", ", requirement.Permissions));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in permission authorization");
        }

        return Task.CompletedTask;
    }
}
