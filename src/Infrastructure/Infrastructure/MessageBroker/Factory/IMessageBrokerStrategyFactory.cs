using Infrastructure.MessageBroker.Abstractions;
using Infrastructure.MessageBroker.Strategies;

namespace Infrastructure.MessageBroker.Factory;

/// <summary>
/// Message broker stratejisi oluşturmak için factory interface
/// </summary>
public interface IMessageBrokerStrategyFactory
{
    /// <summary>
    /// İstenilen message broker stratejisini oluşturur
    /// </summary>
    IMessageBrokerStrategy CreateStrategy(MessageBrokerStrategy strategy);
}