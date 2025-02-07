using System.Linq.Expressions;
using Domain.Common.Abstractions;
using Domain.Common.Audit;
using Infrastructure.Identity.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

/// <summary>
/// Ana veritabanı context sınıfı
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Audit logları için DbSet
    /// </summary>
    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator,
        ICurrentUserService currentUserService) 
        : base(options)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tüm configuration'ları otomatik olarak register et
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Soft delete için global query filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Entity tipi BaseEntity'den türemiş mi kontrol et
            if (typeof(BaseEntity<>).IsAssignableFrom(entityType.ClrType))
            {
                // Soft delete filter'ı ekle
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var property = Expression.Property(parameter, "IsDeleted");
                var falseConstant = Expression.Constant(false);
                var lambda = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
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
    private async Task<List<AuditEntry>> OnBeforeSaveChanges()
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
                UserId = _currentUserService.UserId
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

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.ChangedColumns.Add(propertyName);
                        }
                        break;
                }
            }
        }

        return auditEntries;
    }

    /// <summary>
    /// SaveChanges sonrası işlemler
    /// </summary>
    private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        if (auditEntries == null || !auditEntries.Any())
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
            .Where(x => x.DomainEvents.Any())
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
            await _mediator.Publish(domainEvent);
        }
    }
}