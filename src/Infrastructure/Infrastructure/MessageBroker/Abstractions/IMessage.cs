namespace Infrastructure.MessageBroker.Abstractions;

/// <summary>
/// Message tipi için marker interface
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Mesajın benzersiz ID'si
    /// </summary>
    Guid MessageId { get; }

    /// <summary>
    /// Mesajın oluşturulma zamanı
    /// </summary>
    DateTime CreatedAt { get; }
}

