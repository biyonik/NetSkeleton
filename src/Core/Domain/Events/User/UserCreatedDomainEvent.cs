using Domain.Common.Events;

namespace Domain.Events.User;

/// <summary>
/// Bir kullanıcı oluşturulduğunda fırlatılan event
/// </summary>
public class UserCreatedDomainEvent(
    Guid userId,
    string email,
    string triggeredBy) : BaseDomainEvent(triggeredBy)
{
    /// <summary>
    /// Oluşturulan kullanıcının ID'si
    /// </summary>
    public Guid UserId { get; } = userId;

    /// <summary>
    /// Kullanıcının email adresi
    /// </summary>
    public string Email { get; } = email;
}