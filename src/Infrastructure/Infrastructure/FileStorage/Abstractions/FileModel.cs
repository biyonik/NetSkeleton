namespace Infrastructure.FileStorage.Abstractions;

/// <summary>
/// Dosya bilgilerini tutan model
/// </summary>
public class FileModel
{
    /// <summary>
    /// Dosyanın benzersiz tanımlayıcısı
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Orijinal dosya adı
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Dosya uzantısı (.jpg, .pdf vb.)
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// MIME tipi
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Dosya boyutu (byte)
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Dosyanın saklandığı path veya URL
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Dosya meta verileri
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}