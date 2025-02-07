namespace Infrastructure.MessageBroker.Strategies;

/// <summary>
/// Message broker stratejileri
/// </summary>
public enum MessageBrokerStrategy
{
    InMemory = 1,
    RabbitMQ = 2
}