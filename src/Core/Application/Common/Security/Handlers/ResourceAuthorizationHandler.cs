using System.Security.Claims;
using Application.Common.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Application.Common.Security.Handlers;

/// <summary>
/// Resource bazlı yetkilendirme handler'ı
/// </summary>
public class ResourceAuthorizationHandler(ILogger<ResourceAuthorizationHandler> logger)
    : AuthorizationHandler<ResourceOperationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOperationRequirement requirement)
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

            // Resource bazlı permission'ı kontrol et
            var resourcePermission = $"{requirement.Resource}.{requirement.Operation}";
            var hasPermission = user.Claims
                .Any(c => c.Type == "Permission" && c.Value == resourcePermission);

            if (hasPermission)
            {
                context.Succeed(requirement);
                logger.LogInformation(
                    "User {UserId} authorized for resource {Resource} operation {Operation}",
                    user.FindFirstValue(ClaimTypes.NameIdentifier),
                    requirement.Resource,
                    requirement.Operation);
            }
            else
            {
                logger.LogWarning(
                    "User {UserId} does not have permission for resource {Resource} operation {Operation}",
                    user.FindFirstValue(ClaimTypes.NameIdentifier),
                    requirement.Resource,
                    requirement.Operation);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in resource authorization");
        }

        return Task.CompletedTask;
    }
}