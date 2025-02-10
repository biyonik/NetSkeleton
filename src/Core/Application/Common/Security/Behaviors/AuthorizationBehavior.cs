using System.Reflection;
using Application.Authorization.Services;
using Application.Common.Security.Attributes;
using Application.Common.Security.Exceptions;
using Domain.Authorization.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace Application.Common.Security.Behaviors;

//// <summary>
/// Permission tabanlı authorization pipeline behavior'u
/// </summary>
public class AuthorizationBehavior<TRequest, TResponse>(
    IAuthorizationRepository authorizationRepository,
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
        // Permission ve resource attribute'larını kontrol et
        var attributes = request.GetType().GetCustomAttributes()
            .Where(a => a is RequirePermissionAttribute or ResourceAuthorizationAttribute)
            .ToArray();

        if (attributes.Length == 0)
        {
            return await next();
        }

        // Kullanıcı kontrolü
        if (currentUserService.UserId == null)
        {
            logger.LogWarning("Unauthorized access attempt to {RequestType}", typeof(TRequest).Name);
            throw new UnauthorizedAccessException();
        }

        foreach (var attribute in attributes)
        {
            bool isAuthorized = attribute switch
            {
                RequirePermissionAttribute permissionAttr => 
                    await CheckPermissionAsync(permissionAttr),
                
                ResourceAuthorizationAttribute resourceAttr => 
                    await CheckResourceAccessAsync(resourceAttr),
                
                _ => false
            };

            if (!isAuthorized)
            {
                logger.LogWarning(
                    "User {UserId} not authorized for {AttributeType} on {RequestType}",
                    currentUserService.UserId,
                    attribute.GetType().Name,
                    typeof(TRequest).Name);
                
                throw new ForbiddenAccessException();
            }
        }

        logger.LogInformation(
            "User {UserId} authorized for {RequestType}",
            currentUserService.UserId,
            typeof(TRequest).Name);

        return await next();
    }

    private async Task<bool> CheckPermissionAsync(RequirePermissionAttribute attribute)
    {
        var requiredPermissions = attribute.ToString()
            .Split('_')[1] // "Permission_X,Y,Z" formatından permission'ları al
            .Split(',');

        return await authorizationRepository.HasAnyPermissionAsync(
            currentUserService.UserId!, 
            requiredPermissions);
    }

    private async Task<bool> CheckResourceAccessAsync(ResourceAuthorizationAttribute attribute)
    {
        return await authorizationRepository.CanAccessResourceAsync(
            currentUserService.UserId!,
            attribute.ToString().Split('_')[1], // Resource adı
            attribute.ToString().Split('_')[2]); // Operation adı
    }
}