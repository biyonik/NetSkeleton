using System.Text.Json;
using Infrastructure.Email.Abstractions;
using Infrastructure.Email.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email.Strategies;

/// <summary>
/// Development ortamı için dosya bazlı email stratejisi
/// </summary>
public class FileEmailStrategy : IEmailStrategy
{
    private readonly string _outputDirectory;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileEmailStrategy(IOptions<EmailSettings> settings)
    {
        var fileSettings = settings.Value.FileOutput ?? throw new ArgumentNullException(nameof(settings));
        _outputDirectory = string.IsNullOrWhiteSpace(fileSettings.OutputDirectory)
            ? Path.Combine(Path.GetTempPath(), "emails")
            : fileSettings.OutputDirectory;

        if (!Directory.Exists(_outputDirectory))
            Directory.CreateDirectory(_outputDirectory);
    }

    /// <summary>
    /// Email'i dosya olarak kaydeder
    /// </summary>
    public async Task SendAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        var fileName = $"email_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}.json";
        var filePath = Path.Combine(_outputDirectory, fileName);

        var emailContent = new
        {
            To = email.To,
            Cc = email.Cc,
            Bcc = email.Bcc,
            Subject = email.Subject,
            Body = email.Body,
            IsHtml = email.IsHtml,
            SentDate = DateTime.UtcNow,
            Attachments = email.Attachments.Select(att => new
            {
                att.FileName,
                att.ContentType,
                ContentLength = att.Content.Length
            }).ToList()
        };

        try
        {
            await _semaphore.WaitAsync(cancellationToken);

            var json = JsonSerializer.Serialize(emailContent, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json, cancellationToken);

            // Eğer attachment'lar varsa, onları da kaydet
            if (email.Attachments.Any())
            {
                var attachmentDir = Path.Combine(_outputDirectory, Path.GetFileNameWithoutExtension(fileName));
                Directory.CreateDirectory(attachmentDir);

                foreach (var attachment in email.Attachments)
                {
                    var attachmentPath = Path.Combine(attachmentDir, attachment.FileName);
                    await File.WriteAllBytesAsync(attachmentPath, attachment.Content, cancellationToken);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Toplu email'leri dosya olarak kaydeder
    /// </summary>
    public async Task SendBulkAsync(IEnumerable<EmailMessage> emails, CancellationToken cancellationToken = default)
    {
        foreach (var email in emails)
        {
            await SendAsync(email, cancellationToken);
        }
    }

    /// <summary>
    /// Email çıktı dizinindeki tüm dosyaları siler
    /// Test için kullanışlı
    /// </summary>
    public void ClearOutputDirectory()
    {
        if (Directory.Exists(_outputDirectory))
        {
            Directory.Delete(_outputDirectory, true);
            Directory.CreateDirectory(_outputDirectory);
        }
    }
}