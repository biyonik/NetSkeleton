using Domain.Authorization.Events;
using Domain.Authorization.Repositories;
using Domain.Common.Events;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Application.Authorization.EventHandlers;

public class PermissionGrantCreatedDomainEventHandler(
    IAuthorizationRepository repository,
    ICacheManager cacheManager,
    ILogger<PermissionGrantCreatedDomainEventHandler> logger)
    : IDomainEventHandler<PermissionGrantCreatedDomainEvent>
{
    public async Task Handle(PermissionGrantCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await repository.InvalidateUserPermissionCacheAsync(notification.UserId, cancellationToken);
        logger.LogInformation("Permission grant created: {GrantId} - User: {UserId}, Permission: {PermissionId}", 
            notification.GrantId, notification.UserId, notification.PermissionId);
    }
}