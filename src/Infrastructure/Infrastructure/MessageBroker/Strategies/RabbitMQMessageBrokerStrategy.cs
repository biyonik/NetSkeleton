using System.Text;
using System.Text.Json;
using Infrastructure.MessageBroker.Abstractions;
using Infrastructure.MessageBroker.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Infrastructure.MessageBroker.Strategies;

/// <summary>
/// RabbitMQ message broker implementasyonu
/// </summary>
public class RabbitMQMessageBrokerStrategy : IMessageBrokerStrategy, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Dictionary<string, string> _consumerTags;
    private bool _disposed;

    public RabbitMQMessageBrokerStrategy(IOptions<MessageBrokerSettings> settings)
    {
        var rabbitSettings = settings.Value.RabbitMQ ?? throw new ArgumentNullException(nameof(settings));

        var factory = new ConnectionFactory
        {
            HostName = rabbitSettings.HostName,
            UserName = rabbitSettings.UserName,
            Password = rabbitSettings.Password,
            Port = rabbitSettings.Port,
            VirtualHost = rabbitSettings.VirtualHost,
            AutomaticRecoveryEnabled = rabbitSettings.AutomaticRecoveryEnabled,
            RequestedHeartbeat = rabbitSettings.RequestedHeartbeat
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _consumerTags = new Dictionary<string, string>();
    }

    /// <summary>
    /// Mesaj publish eder
    /// </summary>
    public Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        EnsureTopicExists(topic);

        var messageJson = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageJson);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;  // Mesajlar disk'e yazılsın
        properties.MessageId = message.MessageId.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.Headers = new Dictionary<string, object>
        {
            { "message_type", typeof(TMessage).FullName! }
        };

        _channel.BasicPublish(
            exchange: topic,
            routingKey: string.Empty,
            mandatory: true,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Mesajlara subscribe olur
    /// </summary>
    public Task SubscribeAsync<TMessage>(string topic, Func<TMessage, Task> handler, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        EnsureTopicExists(topic);

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, topic, string.Empty);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<TMessage>(messageJson);

                if (message != null)
                {
                    await handler(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception)
            {
                // Dead letter queue'ya gönder
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        var consumerTag = _channel.BasicConsume(queueName, false, consumer);
        _consumerTags[topic] = consumerTag;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Subscribe'ı iptal eder
    /// </summary>
    public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        if (_consumerTags.TryGetValue(topic, out var consumerTag))
        {
            _channel.BasicCancel(consumerTag);
            _consumerTags.Remove(topic);
        }

        return Task.CompletedTask;
    }

    private void EnsureTopicExists(string topic)
    {
        _channel.ExchangeDeclare(topic, ExchangeType.Fanout, true, false);
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_channel?.IsOpen ?? false)
            _channel?.Close();
        if (_connection?.IsOpen ?? false)
            _connection?.Close();

        _channel?.Dispose();
        _connection?.Dispose();

        _disposed = true;
    }
}