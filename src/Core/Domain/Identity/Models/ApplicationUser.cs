using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Models;

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

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    public virtual ICollection<ApplicationUserClaim> Claims { get; set; } = new List<ApplicationUserClaim>();
    public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; } = new List<IdentityUserLogin<Guid>>();
    public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; } = new List<IdentityUserToken<Guid>>();
}