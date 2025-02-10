using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Models;

/// <summary>
/// Kullanıcı claim sınıfı
/// </summary>
public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    /// <summary>
    /// Claim'in açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Claim'in oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Claim'i oluşturan kullanıcı ID'si
    /// </summary>
    public Guid? CreatedBy { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}