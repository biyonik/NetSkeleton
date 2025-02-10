using Domain.Authorization.Events;
using Domain.Authorization.Repositories;
using Domain.Common.Events;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Application.Authorization.EventHandlers;

public class PermissionClaimsUpdatedDomainEventHandler(
    IAuthorizationRepository repository,
    ICacheManager cacheManager,
    ILogger<PermissionClaimsUpdatedDomainEventHandler> logger)
    : IDomainEventHandler<PermissionClaimsUpdatedDomainEvent>
{
    public async Task Handle(PermissionClaimsUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await repository.InvalidatePermissionCacheAsync(notification.PermissionId, cancellationToken);
        logger.LogInformation("Permission claims updated: {PermissionId} - Claims: {Claims}", 
            notification.PermissionId, notification.Claims);
    }
}