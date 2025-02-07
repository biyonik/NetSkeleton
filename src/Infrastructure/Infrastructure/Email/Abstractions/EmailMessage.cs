namespace Infrastructure.Email.Abstractions;

/// <summary>
/// Email içeriği için model sınıfı
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Alıcı email adresleri
    /// </summary>
    public List<string> To { get; set; } = new();

    /// <summary>
    /// CC email adresleri
    /// </summary>
    public List<string> Cc { get; set; } = new();

    /// <summary>
    /// BCC email adresleri
    /// </summary>
    public List<string> Bcc { get; set; } = new();

    /// <summary>
    /// Email konusu
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Email içeriği
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// HTML formatında mı?
    /// </summary>
    public bool IsHtml { get; set; }

    /// <summary>
    /// Ekler
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = new();
}