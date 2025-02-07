using Infrastructure.MessageBroker.Abstractions;
using Infrastructure.MessageBroker.Factory;
using Infrastructure.MessageBroker.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.MessageBroker;

/// <summary>
/// Message broker manager implementasyonu
/// Seçilen stratejiye göre mesajlaşma işlemlerini yönetir
/// </summary>
public class MessageBrokerManager(
    IMessageBrokerStrategyFactory strategyFactory,
    IOptions<MessageBrokerSettings> settings)
    : IMessageBrokerManager, IDisposable
{
    private readonly IMessageBrokerStrategy _messageBrokerStrategy = strategyFactory.CreateStrategy(settings.Value.Strategy);
    private bool _disposed;

    /// <summary>
    /// Mesaj publish eder
    /// </summary>
    public async Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        try
        {
            await _messageBrokerStrategy.PublishAsync(topic, message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            // Retry policy uygulanabilir
            throw;
        }
    }

    /// <summary>
    /// Mesajlara subscribe olur
    /// </summary>
    public async Task SubscribeAsync<TMessage>(string topic, Func<TMessage, Task> handler, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        try
        {
            await _messageBrokerStrategy.SubscribeAsync(topic, handler, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    /// <summary>
    /// Subscribe'ı iptal eder
    /// </summary>
    public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        try
        {
            await _messageBrokerStrategy.UnsubscribeAsync(topic, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_messageBrokerStrategy is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _disposed = true;
    }
}