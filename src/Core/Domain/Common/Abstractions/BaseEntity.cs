using MediatR;

namespace Domain.Common.Abstractions;


/// <summary>
/// Tüm entity'ler için temel sınıf.
/// Audit özellikleri entegre edilmiş hali.
/// </summary>
/// <typeparam name="TKey">Entity'nin primary key tipi</typeparam>
public abstract class BaseEntity<TKey>() : IEntity<TKey>
    where TKey : struct
{
    private readonly List<INotification> _domainEvents = new();
    
    /// <summary>
    /// Entity'nin benzersiz tanımlayıcısı
    /// </summary>
    public TKey Id { get; protected set; }

    public bool IsActive { get; } = true;

    #region Audit Properties
    /// <summary>
    /// Kaydı oluşturan kullanıcı
    /// </summary>
    public string? CreatedBy { get; protected set; }

    /// <summary>
    /// Kaydın oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Kaydı son güncelleyen kullanıcı
    /// </summary>
    public string? LastModifiedBy { get; protected set; }

    /// <summary>
    /// Kaydın son güncellenme tarihi
    /// </summary>
    public DateTime? LastModifiedDate { get; protected set; }

    /// <summary>
    /// Kaydın silinme durumu
    /// </summary>
    public bool IsDeleted { get; protected set; } = false;

    /// <summary>
    /// Kaydı silen kullanıcı
    /// </summary>
    public string? DeletedBy { get; protected set; }

    /// <summary>
    /// Kaydın silinme tarihi
    /// </summary>
    public DateTime? DeletedDate { get; protected set; }

    /// <summary>
    /// Versiyon bilgisi - Concurrency için
    /// </summary>
    public int Version { get; protected set; } = 1;

    #endregion

    /// <summary>
    /// Entity üzerinde gerçekleşen domain event'leri
    /// </summary>
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity(TKey id) : this()
    {
        Id = id;
    }

    /// <summary>
    /// Entity'ye yeni bir domain event ekler
    /// </summary>
    protected void AddDomainEvent(INotification eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    /// <summary>
    /// Entity'den bir domain event'i kaldırır
    /// </summary>
    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    /// <summary>
    /// Tüm domain event'leri temizler
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Entity'yi günceller ve audit bilgilerini set eder
    /// </summary>
    protected void Update(string modifiedBy)
    {
        LastModifiedBy = modifiedBy;
        LastModifiedDate = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Entity'yi soft-delete yapar
    /// </summary>
    public virtual void Delete(string deletedBy)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedDate = DateTime.UtcNow;
        Update(deletedBy);
    }

    /// <summary>
    /// Soft-delete'i geri alır
    /// </summary>
    public virtual void Restore(string restoredBy)
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedBy = null;
        DeletedDate = null;
        Update(restoredBy);
    }
}