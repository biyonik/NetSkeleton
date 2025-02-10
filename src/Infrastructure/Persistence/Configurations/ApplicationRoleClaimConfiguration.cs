using Domain.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

/// <summary>
/// ApplicationRoleClaim entity konfigürasyonu
/// </summary>
public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder.ToTable("RoleClaims");

        // Property'ler
        builder.Property(rc => rc.Description)
            .HasMaxLength(500);

        builder.Property(rc => rc.Group)
            .HasMaxLength(100);

        builder.Property(rc => rc.CreatedDate)
            .IsRequired();

        builder.Property(rc => rc.CreatedBy)
            .HasMaxLength(450);
    }
}