using MediatR;

namespace Domain.Common.Events;

/// <summary>
/// Domain event handler'ları için marker interface.
/// MediatR'ın INotificationHandler'ını extend eder.
/// </summary>
/// <typeparam name="TDomainEvent">İşlenecek domain event tipi</typeparam>
public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : BaseDomainEvent
{
}