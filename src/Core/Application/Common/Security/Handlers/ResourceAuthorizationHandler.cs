using System.Security.Claims;
using Application.Common.Security.Authorization;
using Domain.Authorization.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Application.Common.Security.Handlers;

/// <summary>
/// Resource bazlı yetkilendirme handler'ı
/// </summary>
public class ResourceAuthorizationHandler(
    IAuthorizationRepository authorizationRepository,
    ILogger<ResourceAuthorizationHandler> logger)
    : AuthorizationHandler<ResourceOperationRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOperationRequirement requirement)
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

            var hasAccess = await authorizationRepository.CanAccessResourceAsync(
                userId, 
                requirement.Resource, 
                requirement.Operation);

            if (hasAccess)
            {
                context.Succeed(requirement);
                logger.LogInformation(
                    "User {UserId} authorized for resource {Resource} operation {Operation}",
                    userId,
                    requirement.Resource,
                    requirement.Operation);
            }
            else
            {
                // Context varsa, detaylı kontrol yap
                if (requirement.Context != null)
                {
                    var (hasContextAccess, reason) = await authorizationRepository.EvaluateAccessAsync(
                        userId,
                        $"{requirement.Resource}.{requirement.Operation}",
                        System.Text.Json.JsonSerializer.Serialize(requirement.Context));

                    if (hasContextAccess)
                    {
                        context.Succeed(requirement);
                        return;
                    }

                    logger.LogWarning(
                        "User {UserId} access denied for resource {Resource} operation {Operation}. Reason: {Reason}",
                        userId,
                        requirement.Resource,
                        requirement.Operation,
                        reason);
                }
                else
                {
                    logger.LogWarning(
                        "User {UserId} does not have permission for resource {Resource} operation {Operation}",
                        userId,
                        requirement.Resource,
                        requirement.Operation);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in resource authorization");
        }
    }
}