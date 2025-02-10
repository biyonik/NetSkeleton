using Domain.Authorization.Events;
using Domain.Authorization.Repositories;
using Domain.Common.Events;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Application.Authorization.EventHandlers;

public class PermissionGrantDeactivatedDomainEventHandler(
    IAuthorizationRepository repository,
    ICacheManager cacheManager,
    ILogger<PermissionGrantDeactivatedDomainEventHandler> logger)
    : IDomainEventHandler<PermissionGrantDeactivatedDomainEvent>
{
    private readonly ICacheManager _cacheManager = cacheManager;

    public async Task Handle(PermissionGrantDeactivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await repository.InvalidateUserPermissionCacheAsync(notification.UserId, cancellationToken);
        logger.LogInformation("Permission grant deactivated: {GrantId} - User: {UserId}, Permission: {PermissionId}", 
            notification.GrantId, notification.UserId, notification.PermissionId);
    }
}