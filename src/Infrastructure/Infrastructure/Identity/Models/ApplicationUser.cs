using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models;

/// <summary>
/// Uygulama kullanıcı sınıfı
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Kullanıcının adı
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının soyadı
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının tam adı
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Kullanıcının refresh token'ı
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token'ın son kullanma tarihi
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    /// <summary>
    /// Kullanıcının son aktivite tarihi
    /// </summary>
    public DateTime? LastActivityDate { get; set; }

    /// <summary>
    /// Kullanıcının fotoğraf URL'i
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Kullanıcının aktif olup olmadığı
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Kullanıcının oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Kullanıcıyı oluşturan kullanıcı ID'si
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Kullanıcının son güncelleme tarihi
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }

    /// <summary>
    /// Kullanıcıyı son güncelleyen kullanıcı ID'si
    /// </summary>
    public Guid? LastModifiedBy { get; set; }

    /// <summary>
    /// Kullanıcının özel ayarları
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new();
}