using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models;

/// <summary>
/// Uygulama rol sınıfı
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>
    /// Rolün açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Rolün oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Rolü oluşturan kullanıcı ID'si
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Rolün son güncelleme tarihi
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }

    /// <summary>
    /// Rolü son güncelleyen kullanıcı ID'si
    /// </summary>
    public Guid? LastModifiedBy { get; set; }
}