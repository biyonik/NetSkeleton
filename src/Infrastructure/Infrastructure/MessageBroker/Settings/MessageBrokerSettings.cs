using Infrastructure.MessageBroker.Strategies;

namespace Infrastructure.MessageBroker.Settings;

/// <summary>
/// Message broker ayarları
/// </summary>
public class MessageBrokerSettings
{
    /// <summary>
    /// Kullanılacak message broker stratejisi
    /// </summary>
    public MessageBrokerStrategy Strategy { get; set; } = MessageBrokerStrategy.InMemory;

    /// <summary>
    /// RabbitMQ ayarları
    /// </summary>
    public RabbitMQSettings? RabbitMQ { get; set; }
}