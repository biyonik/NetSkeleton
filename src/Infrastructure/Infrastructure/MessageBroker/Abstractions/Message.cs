namespace Infrastructure.MessageBroker.Abstractions;

/// <summary>
/// Temel mesaj sınıfı
/// </summary>
public abstract class Message : IMessage
{
    public Guid MessageId { get; }
    public DateTime CreatedAt { get; }

    protected Message()
    {
        MessageId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}