using Infrastructure.Email.Abstractions;

namespace Infrastructure.Email;

/// <summary>
/// Email işlemlerini yöneten manager interface
/// </summary>
public interface IEmailManager
{
    Task SendAsync(EmailMessage email, CancellationToken cancellationToken = default);
    Task SendBulkAsync(IEnumerable<EmailMessage> emails, CancellationToken cancellationToken = default);
    Task SendTemplatedEmailAsync(string templateName, object model, EmailMessage email, CancellationToken cancellationToken = default);
}