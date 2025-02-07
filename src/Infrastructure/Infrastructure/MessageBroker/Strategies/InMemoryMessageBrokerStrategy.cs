using Infrastructure.MessageBroker.Abstractions;

namespace Infrastructure.MessageBroker.Strategies;

/// <summary>
/// Development ortamı için in-memory message broker implementasyonu
/// </summary>
public class InMemoryMessageBrokerStrategy : IMessageBrokerStrategy
{
    private readonly Dictionary<string, List<Delegate>> _handlers = new();
    private readonly object _lock = new();

    /// <summary>
    /// Mesaj publish eder
    /// </summary>
    public async Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentNullException(nameof(topic));

        List<Delegate>? topicHandlers;
        lock (_lock)
        {
            _handlers.TryGetValue(topic, out topicHandlers);
        }

        if (topicHandlers == null || !topicHandlers.Any())
            return;

        foreach (var handler in topicHandlers.ToList())
        {
            try
            {
                if (handler is Func<TMessage, Task> messageHandler)
                {
                    await messageHandler(message);
                }
            }
            catch (Exception ex)
            {
                // Log error
                // Development ortamında olduğumuz için exception'ı fırlatabiliriz
                throw;
            }
        }
    }

    /// <summary>
    /// Mesajlara subscribe olur
    /// </summary>
    public Task SubscribeAsync<TMessage>(string topic, Func<TMessage, Task> handler, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentNullException(nameof(topic));

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        lock (_lock)
        {
            if (!_handlers.ContainsKey(topic))
            {
                _handlers[topic] = new List<Delegate>();
            }

            _handlers[topic].Add(handler);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Subscribe'ı iptal eder
    /// </summary>
    public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentNullException(nameof(topic));

        lock (_lock)
        {
            if (_handlers.ContainsKey(topic))
            {
                _handlers.Remove(topic);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Tüm handler'ları temizler
    /// Test ve debug için kullanışlı
    /// </summary>
    public void ClearAllHandlers()
    {
        lock (_lock)
        {
            _handlers.Clear();
        }
    }

    /// <summary>
    /// Belirli bir topic için kayıtlı handler sayısını döner
    /// Test ve debug için kullanışlı
    /// </summary>
    public int GetHandlerCount(string topic)
    {
        lock (_lock)
        {
            return _handlers.TryGetValue(topic, out var handlers) ? handlers.Count : 0;
        }
    }
}
