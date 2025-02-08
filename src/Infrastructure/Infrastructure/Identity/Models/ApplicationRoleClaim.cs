using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models;

/// <summary>
/// Rol claim sınıfı
/// </summary>
public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    /// <summary>
    /// Claim'in açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Claim grubu
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Claim'in oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Claim'i oluşturan kullanıcı ID'si
    /// </summary>
    public Guid? CreatedBy { get; set; }
}