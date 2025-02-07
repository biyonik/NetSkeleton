using Infrastructure.MessageBroker.Factory;
using Infrastructure.MessageBroker.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MessageBroker.Extensions;

/// <summary>
/// Message broker servislerinin registrasyonu için extension methods
/// </summary>
public static class MessageBrokerServiceExtensions
{
    /// <summary>
    /// Message broker servislerini DI container'a register eder
    /// </summary>
    public static IServiceCollection AddMessageBroker(
        this IServiceCollection services,
        Action<MessageBrokerSettings> configureOptions)
    {
        // Settings'i configure et
        services.Configure(configureOptions);

        // Factory ve Manager'ı register et
        services.AddSingleton<IMessageBrokerStrategyFactory, MessageBrokerStrategyFactory>();
        services.AddSingleton<IMessageBrokerManager, MessageBrokerManager>();

        return services;
    }
}