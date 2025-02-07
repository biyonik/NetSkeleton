using Infrastructure.MessageBroker.Abstractions;
using Infrastructure.MessageBroker.Settings;
using Infrastructure.MessageBroker.Strategies;
using Microsoft.Extensions.Options;

namespace Infrastructure.MessageBroker.Factory;

/// <summary>
/// Message broker factory implementasyonu
/// </summary>
public class MessageBrokerStrategyFactory(IOptions<MessageBrokerSettings> settings) : IMessageBrokerStrategyFactory
{
    /// <summary>
    /// Verilen strateji tipine göre message broker implementasyonu oluşturur
    /// </summary>
    public IMessageBrokerStrategy CreateStrategy(MessageBrokerStrategy strategy)
    {
        return strategy switch
        {
            MessageBrokerStrategy.InMemory => new InMemoryMessageBrokerStrategy(),
            MessageBrokerStrategy.RabbitMQ => new RabbitMQMessageBrokerStrategy(settings),
            _ => throw new ArgumentException($"Unsupported message broker strategy: {strategy}")
        };
    }
}