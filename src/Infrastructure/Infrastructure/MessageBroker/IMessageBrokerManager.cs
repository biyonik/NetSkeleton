using Infrastructure.MessageBroker.Abstractions;

namespace Infrastructure.MessageBroker;

/// <summary>
/// Message broker işlemlerini yöneten manager interface
/// </summary>
public interface IMessageBrokerManager
{
    Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage;
        
    Task SubscribeAsync<TMessage>(string topic, Func<TMessage, Task> handler, CancellationToken cancellationToken = default)
        where TMessage : IMessage;
        
    Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);
}
