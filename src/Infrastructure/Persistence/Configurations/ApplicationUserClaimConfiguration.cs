using Domain.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

/// <summary>
/// ApplicationUserClaim entity konfigürasyonu
/// </summary>
public class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
    {
        builder.ToTable("UserClaims");

        // Property'ler
        builder.Property(uc => uc.Description)
            .HasMaxLength(500);

        builder.Property(uc => uc.CreatedDate)
            .IsRequired();

        builder.Property(uc => uc.CreatedBy)
            .HasMaxLength(450);
    }
}