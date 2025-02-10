using Domain.Authorization.Events;
using Domain.Authorization.Repositories;
using Domain.Common.Events;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Application.Authorization.EventHandlers;

public class PermissionGrantUpdatedDomainEventHandler(
    IAuthorizationRepository repository,
    ICacheManager cacheManager,
    ILogger<PermissionGrantUpdatedDomainEventHandler> logger)
    : IDomainEventHandler<PermissionGrantUpdatedDomainEvent>
{
    public async Task Handle(PermissionGrantUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await repository.InvalidateUserPermissionCacheAsync(notification.UserId, cancellationToken);
        logger.LogInformation("Permission grant updated: {GrantId} - User: {UserId}, Permission: {PermissionId}", 
            notification.GrantId, notification.UserId, notification.PermissionId);
    }
}