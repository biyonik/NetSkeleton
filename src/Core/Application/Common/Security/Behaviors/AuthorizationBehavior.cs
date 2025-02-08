using System.Reflection;
using Application.Common.Security.Exceptions;
using Infrastructure.Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Application.Common.Security.Behaviors;

/// <summary>
/// Authorization pipeline behavior'u
/// </summary>
public class AuthorizationBehavior<TRequest, TResponse>(
    IAuthorizationService authorizationService,
    ICurrentUserService currentUserService,
    ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Request'in attribute'larını kontrol et
        var authorizeAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>();

        var attributes = authorizeAttributes as AuthorizeAttribute[] ?? authorizeAttributes.ToArray();
        if (attributes.Length != 0)
        {
            // Kullanıcı kontrolü
            if (currentUserService.UserId == null)
            {
                logger.LogWarning("Unauthorized access attempt to {RequestType}", typeof(TRequest).Name);
                throw new UnauthorizedAccessException();
            }

            // Her bir attribute için yetki kontrolü yap
            foreach (var attribute in attributes)
            {
                var policy = attribute.Policy;
                if (!string.IsNullOrEmpty(policy))
                {
                    var authorized = await EvaluatePolicyAsync(policy);
                    if (!authorized)
                    {
                        logger.LogWarning(
                            "User {UserId} not authorized for policy {Policy} on {RequestType}",
                            currentUserService.UserId,
                            policy,
                            typeof(TRequest).Name);
                        throw new ForbiddenAccessException();
                    }
                }

                var roles = attribute.Roles?.Split(',');
                if (roles?.Any() == true)
                {
                    var authorized = await EvaluateRolesAsync(roles);
                    if (!authorized)
                    {
                        logger.LogWarning(
                            "User {UserId} does not have required roles {Roles} for {RequestType}",
                            currentUserService.UserId,
                            string.Join(", ", roles),
                            typeof(TRequest).Name);
                        throw new ForbiddenAccessException();
                    }
                }
            }

            logger.LogInformation(
                "User {UserId} authorized for {RequestType}",
                currentUserService.UserId,
                typeof(TRequest).Name);
        }

        // Pipeline'a devam et
        return await next();
    }

    private async Task<bool> EvaluatePolicyAsync(string policy)
    {
        var result = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            null,
            policy);

        return result.Succeeded;
    }

    private async Task<bool> EvaluateRolesAsync(string[] roles)
    {
        foreach (var role in roles)
        {
            var result = await authorizationService.AuthorizeAsync(
                currentUserService.User,
                null,
                $"Role_{role.Trim()}");

            if (result.Succeeded)
                return true;
        }

        return false;
    }
}