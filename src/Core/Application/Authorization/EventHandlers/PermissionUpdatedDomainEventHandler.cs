using Domain.Authorization.Events;
using Domain.Authorization.Repositories;
using Domain.Common.Events;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Application.Authorization.EventHandlers;

public class PermissionUpdatedDomainEventHandler(
    IAuthorizationRepository repository,
    ICacheManager cacheManager,
    ILogger<PermissionUpdatedDomainEventHandler> logger)
    : IDomainEventHandler<PermissionUpdatedDomainEvent>
{
    public async Task Handle(PermissionUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await repository.InvalidatePermissionCacheAsync(notification.PermissionId, cancellationToken);
        logger.LogInformation("Permission updated: {PermissionId} - {SystemName}", 
            notification.PermissionId, notification.SystemName);
    }
}