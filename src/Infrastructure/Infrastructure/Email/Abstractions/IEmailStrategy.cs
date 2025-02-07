namespace Infrastructure.Email.Abstractions;

/// <summary>
/// Email gönderim stratejileri için temel interface
/// </summary>
public interface IEmailStrategy
{
    /// <summary>
    /// Email gönderir
    /// </summary>
    Task SendAsync(EmailMessage email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toplu email gönderir
    /// </summary>
    Task SendBulkAsync(IEnumerable<EmailMessage> emails, CancellationToken cancellationToken = default);
}