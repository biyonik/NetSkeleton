using Infrastructure.Email.Abstractions;
using Infrastructure.Email.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email.Strategies;

/// <summary>
/// SMTP tabanlı email stratejisi
/// </summary>
public class SmtpEmailStrategy : IEmailStrategy, IDisposable
{
    private readonly SmtpSettings _smtpSettings;
    private readonly EmailSettings _emailSettings;
    private bool _disposed;

    public SmtpEmailStrategy(IOptions<EmailSettings> settings)
    {
        _emailSettings = settings.Value;
        _smtpSettings = settings.Value.Smtp ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// SMTP üzerinden email gönderir
    /// </summary>
    public async Task SendAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        var message = CreateMimeMessage(email);

        using var client = new SmtpClient();
        
        try
        {
            await client.ConnectAsync(
                _smtpSettings.Host,
                _smtpSettings.Port,
                _smtpSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            if (!string.IsNullOrEmpty(_smtpSettings.UserName))
            {
                await client.AuthenticateAsync(
                    _smtpSettings.UserName,
                    _smtpSettings.Password,
                    cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken);
        }
    }

    /// <summary>
    /// Toplu email gönderir
    /// </summary>
    public async Task SendBulkAsync(IEnumerable<EmailMessage> emails, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient();
        
        try
        {
            await client.ConnectAsync(
                _smtpSettings.Host,
                _smtpSettings.Port,
                _smtpSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            if (!string.IsNullOrEmpty(_smtpSettings.UserName))
            {
                await client.AuthenticateAsync(
                    _smtpSettings.UserName,
                    _smtpSettings.Password,
                    cancellationToken);
            }

            foreach (var email in emails)
            {
                var message = CreateMimeMessage(email);
                await client.SendAsync(message, cancellationToken);
            }
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken);
        }
    }

    /// <summary>
    /// MimeMessage oluşturur
    /// </summary>
    private MimeMessage CreateMimeMessage(EmailMessage email)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _emailSettings.DefaultFromName,
            _emailSettings.DefaultFromEmail));

        message.To.AddRange(email.To.Select(x => new MailboxAddress("", x)));
        message.Cc.AddRange(email.Cc.Select(x => new MailboxAddress("", x)));
        message.Bcc.AddRange(email.Bcc.Select(x => new MailboxAddress("", x)));

        message.Subject = email.Subject;

        var builder = new BodyBuilder();
        if (email.IsHtml)
            builder.HtmlBody = email.Body;
        else
            builder.TextBody = email.Body;

        // Attachment'ları ekle
        foreach (var attachment in email.Attachments)
        {
            builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
        }

        message.Body = builder.ToMessageBody();
        return message;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}