using System.Security.Claims;
using Application.Common.Security.Abstractions;
using Application.Common.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Application.Common.Security.Handlers;

/// <summary>
/// Dinamik policy handler'ı
/// </summary>
public class DynamicPolicyHandler(
    ILogger<DynamicPolicyHandler> logger,
    IDynamicPolicyService policyService)
    : AuthorizationHandler<DynamicPolicyRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DynamicPolicyRequirement requirement)
    {
        try
        {
            var user = context.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                logger.LogWarning("User is not authenticated");
                return;
            }

            // Super admin her zaman erişebilir
            if (user.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAuthorized = await policyService.EvaluatePolicyAsync(
                requirement.PolicyName,
                userId);

            if (isAuthorized)
            {
                context.Succeed(requirement);
                logger.LogInformation(
                    "User {UserId} authorized for policy {Policy}",
                    userId,
                    requirement.PolicyName);
            }
            else
            {
                logger.LogWarning(
                    "User {UserId} not authorized for policy {Policy}",
                    userId,
                    requirement.PolicyName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in dynamic policy authorization");
        }
    }
}