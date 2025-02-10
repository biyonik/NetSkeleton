using System.Linq.Expressions;
using Application.Authorization.Services;
using Domain.Authorization;
using Domain.Common.Abstractions;
using Domain.Common.Audit;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

/// <summary>
/// Ana veritabanı context sınıfı
/// </summary>
public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUserService)
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        ApplicationUserClaim,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        ApplicationRoleClaim,
        IdentityUserToken<Guid>>(options: options)
{
    /// <summary>
    /// Audit logları için DbSet
    /// </summary>
    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();

    public DbSet<PermissionEndpoint> PermissionEndpoints { get; set; }


    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionGrant> PermissionGrants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tüm configuration'ları otomatik olarak register et
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Soft delete için global query filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Entity tipi BaseEntity'den türemiş mi kontrol et
            if (!typeof(BaseEntity<>).IsAssignableFrom(entityType.ClrType)) continue;
            
            // Soft delete filter'ı ekle
            var parameter = Expression.Parameter(entityType.ClrType, "p");
            var property = Expression.Property(parameter, "IsDeleted");
            var falseConstant = Expression.Constant(false);
            var lambda = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// SaveChanges öncesi ve sonrası işlemler
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Audit trail için değişiklikleri yakala
        var auditEntries = await OnBeforeSaveChanges();

        // Domain eventleri collect et
        var domainEvents = GetDomainEvents();

        // Değişiklikleri kaydet
        var result = await base.SaveChangesAsync(cancellationToken);

        // Audit log kayıtlarını oluştur
        await OnAfterSaveChanges(auditEntries);

        // Domain eventleri publish et
        await DispatchDomainEvents(domainEvents);

        return result;
    }

    /// <summary>
    /// SaveChanges öncesi işlemler
    /// </summary>
    private Task<List<AuditEntry>?> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditTrail || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = currentUserService.UserId
            };
            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry)
                {
                    case { State: EntityState.Added }:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case { State: EntityState.Deleted }:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case { State: EntityState.Modified }:
                        if (property.IsModified)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.ChangedColumns.Add(propertyName);
                        }
                        break;
                    case { State: EntityState.Detached }:
                    case { State: EntityState.Unchanged }:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        return Task.FromResult<List<AuditEntry>?>(auditEntries);
    }

    /// <summary>
    /// SaveChanges sonrası işlemler
    /// </summary>
    private async Task OnAfterSaveChanges(List<AuditEntry>? auditEntries)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return;

        foreach (var auditEntry in auditEntries)
        {
            // Temporary property'ler için değerleri güncelle
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                    auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                else
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }

            // Audit trail kaydı oluştur
            var auditTrail = auditEntry.ToAudit();
            AuditTrails.Add(auditTrail);
        }

        await SaveChangesAsync();
    }

    /// <summary>
    /// Domain eventleri toplar
    /// </summary>
    private List<INotification> GetDomainEvents()
    {
        var domainEvents = ChangeTracker.Entries<BaseEntity<Guid>>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Count != 0)
            .SelectMany(x => x.DomainEvents)
            .ToList();

        return domainEvents;
    }

    /// <summary>
    /// Domain eventleri publish eder
    /// </summary>
    private async Task DispatchDomainEvents(List<INotification> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}