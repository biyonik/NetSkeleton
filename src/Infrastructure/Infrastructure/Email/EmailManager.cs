using Infrastructure.Email.Abstractions;
using Infrastructure.Email.Exceptions;
using Infrastructure.Email.Factory;
using Infrastructure.Email.Settings;
using Infrastructure.Email.Templates;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email;

/// <summary>
/// Email manager implementasyonu
/// Seçilen stratejiye göre email işlemlerini yönetir
/// </summary>
public class EmailManager : IEmailManager, IDisposable
{
    private readonly IEmailStrategy _emailStrategy;
    private readonly ITemplateRenderer _templateRenderer;
    private bool _disposed;

    public EmailManager(
        IEmailStrategyFactory strategyFactory,
        ITemplateRenderer templateRenderer,
        IOptions<EmailSettings> settings)
    {
        _emailStrategy = strategyFactory.CreateStrategy(settings.Value.Strategy);
        _templateRenderer = templateRenderer;
    }

    /// <summary>
    /// Email gönderir
    /// </summary>
    public async Task SendAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateEmail(email);
            await _emailStrategy.SendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            throw new EmailException("Failed to send email", ex);
        }
    }

    /// <summary>
    /// Toplu email gönderir
    /// </summary>
    public async Task SendBulkAsync(IEnumerable<EmailMessage> emails, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var email in emails)
            {
                ValidateEmail(email);
            }

            await _emailStrategy.SendBulkAsync(emails, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            throw new EmailException("Failed to send bulk emails", ex);
        }
    }

    /// <summary>
    /// Template kullanarak email gönderir
    /// </summary>
    public async Task SendTemplatedEmailAsync(
        string templateName, 
        object model, 
        EmailMessage email, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateEmail(email);

            // Template'i render et
            email.Body = await _templateRenderer.RenderAsync(templateName, model);
            email.IsHtml = true;

            await _emailStrategy.SendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error
            throw new EmailException($"Failed to send templated email. Template: {templateName}", ex);
        }
    }

    /// <summary>
    /// Email nesnesini validate eder
    /// </summary>
    private void ValidateEmail(EmailMessage email)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        if (!email.To.Any())
            throw new EmailValidationException("At least one recipient is required");

        foreach (var recipient in email.To.Concat(email.Cc).Concat(email.Bcc))
        {
            if (!IsValidEmail(recipient))
                throw new EmailValidationException($"Invalid email address: {recipient}");
        }

        if (string.IsNullOrWhiteSpace(email.Subject))
            throw new EmailValidationException("Subject is required");

        if (string.IsNullOrWhiteSpace(email.Body))
            throw new EmailValidationException("Body is required");
    }

    /// <summary>
    /// Email adresinin geçerli olup olmadığını kontrol eder
    /// </summary>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_emailStrategy is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _disposed = true;
    }
}