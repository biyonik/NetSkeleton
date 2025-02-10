using Domain.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Models;

/// <summary>
/// Kullanıcı-Rol ilişki sınıfı
/// </summary>
public class ApplicationUserRole : IdentityUserRole<Guid>
{
    /// <summary>
    /// İlişkinin oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// İlişkiyi oluşturan kullanıcı ID'si
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Özel kısıtlamalar (JSON formatında)
    /// </summary>
    public string? Restrictions { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationRole Role { get; set; } = null!;
}