using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.EventHandlers;

/// <summary>
/// AuditTrailCreatedDomainEvent'i işleyen handler
/// </summary>
public class AuditTrailCreatedDomainEventHandler(ILogger<AuditTrailCreatedDomainEventHandler> logger)
    : INotificationHandler<AuditTrailCreatedDomainEvent>
{
    public Task Handle(AuditTrailCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Audit trail created for {TableName}, Record: {RecordId}, User: {UserId}",
                notification.TableName,
                notification.RecordId,
                notification.UserId);

            // Burada ek işlemler yapılabilir:
            // - Elastic Search'e log gönderme
            // - Notification sistemi tetikleme
            // - External sistemlere bilgi gönderme
            // vs.

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "Error handling AuditTrailCreatedDomainEvent for {TableName}, Record: {RecordId}",
                notification.TableName,
                notification.RecordId);
            throw;
        }
    }
}