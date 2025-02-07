namespace Infrastructure.Email.Settings;

/// <summary>
/// Email servisi ayarları
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Kullanılacak email stratejisi
    /// </summary>
    public EmailStrategy Strategy { get; set; } = EmailStrategy.File;

    /// <summary>
    /// SMTP ayarları
    /// </summary>
    public SmtpSettings? Smtp { get; set; }

    /// <summary>
    /// SendGrid ayarları
    /// </summary>
    public SendGridSettings? SendGrid { get; set; }

    /// <summary>
    /// File output ayarları
    /// </summary>
    public FileEmailSettings? FileOutput { get; set; }

    /// <summary>
    /// Varsayılan gönderen email adresi
    /// </summary>
    public string DefaultFromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Varsayılan gönderen adı
    /// </summary>
    public string DefaultFromName { get; set; } = string.Empty;
}