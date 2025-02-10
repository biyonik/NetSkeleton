using Domain.Authorization.Events;
using Domain.Authorization.Repositories;
using Domain.Common.Events;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Application.Authorization.EventHandlers;

public class PermissionCreatedDomainEventHandler(
    IAuthorizationRepository repository,
    ICacheManager cacheManager,
    ILogger<PermissionCreatedDomainEventHandler> logger)
    : IDomainEventHandler<PermissionCreatedDomainEvent>
{
    private readonly ICacheManager _cacheManager = cacheManager;

    public async Task Handle(PermissionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await repository.InvalidateAllPermissionCachesAsync(cancellationToken);
        logger.LogInformation("Permission created: {PermissionId} - {SystemName}", 
            notification.PermissionId, notification.SystemName);
    }
}