namespace Infrastructure.MessageBroker.Abstractions;

/// <summary>
/// Message broker stratejileri için temel interface
/// </summary>
public interface IMessageBrokerStrategy
{
    /// <summary>
    /// Mesaj publish eder
    /// </summary>
    Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;

    /// <summary>
    /// Mesajlara subscribe olur
    /// </summary>
    Task SubscribeAsync<TMessage>(string topic, Func<TMessage, Task> handler, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;

    /// <summary>
    /// Subscribe'ı iptal eder
    /// </summary>
    Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);
}