namespace Infrastructure.Email.Abstractions;

/// <summary>
/// Email eki için model sınıfı
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// Dosya adı
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Dosya içeriği
    /// </summary>
    public byte[] Content { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// MIME tipi
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
}