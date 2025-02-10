using Domain.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

/// <summary>
/// ApplicationUserRole entity konfigürasyonu
/// </summary>
public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.ToTable("UserRoles");

        // Composite key
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // Property'ler
        builder.Property(ur => ur.CreatedDate)
            .IsRequired();

        builder.Property(ur => ur.CreatedBy)
            .HasMaxLength(450);

        builder.Property(ur => ur.Restrictions)
            .HasColumnType("jsonb"); // PostgreSQL için
    }
}