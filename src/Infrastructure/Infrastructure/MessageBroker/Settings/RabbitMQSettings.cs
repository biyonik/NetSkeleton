namespace Infrastructure.MessageBroker.Settings;

/// <summary>
/// RabbitMQ ayarları
/// </summary>
public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public bool AutomaticRecoveryEnabled { get; set; } = true;
    public TimeSpan RequestedHeartbeat { get; set; } = TimeSpan.FromSeconds(60);
}