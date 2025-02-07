namespace Domain.Common.Abstractions;

/// <summary>
/// Tüm Aggregate Root'lar için temel sınıf.
/// Bu sınıf, DDD'nin Aggregate Root pattern'ini implemente eder.
/// </summary>
/// <typeparam name="TKey">Aggregate root'un primary key tipi</typeparam>
public abstract class BaseAggregateRoot<TKey> : BaseEntity<TKey>, IAggregateRoot<TKey> 
    where TKey : struct
{
    /// <summary>
    /// Aggregate'in versiyon numarası.
    /// Her değişiklikte otomatik artar.
    /// Optimistic concurrency kontrolü için kullanılır.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Aggregate üzerinde yapılan değişikliklerin sayısı.
    /// Unit of Work pattern'de transaction yönetimi için kullanılır.
    /// </summary>
    public int PendingChanges { get; private set; }

    /// <summary>
    /// Aggregate üzerindeki değişiklik sayacı.
    /// Bu sayaç, performans optimizasyonu için kullanılır.
    /// </summary>
    private int _changeCount;

    protected BaseAggregateRoot()
    {
        Version = 1;
        PendingChanges = 0;
        _changeCount = 0;
    }

    protected BaseAggregateRoot(TKey id) : base(id)
    {
        Version = 1;
        PendingChanges = 0;
        _changeCount = 0;
    }

    /// <summary>
    /// Aggregate üzerinde bir değişiklik yapıldığını işaretler.
    /// Bu metod, her state değişikliğinde çağrılmalıdır.
    /// </summary>
    /// <param name="modifiedBy">Değişikliği yapan kullanıcı</param>
    protected void RegisterChange(string modifiedBy)
    {
        _changeCount++;
        PendingChanges = _changeCount;
        Version++;
        base.Update(modifiedBy);
    }

    /// <summary>
    /// Aggregate üzerindeki değişikliklerin kaydedildiğini işaretler.
    /// Bu metod, Unit of Work tarafından commit sonrası çağrılır.
    /// </summary>
    public void CommitChanges()
    {
        _changeCount = 0;
        PendingChanges = 0;
    }

    /// <summary>
    /// Aggregate üzerindeki değişiklikleri geri alır.
    /// Bu metod, transaction rollback durumunda çağrılır.
    /// </summary>
    public void RollbackChanges()
    {
        _changeCount = 0;
        PendingChanges = 0;
        Version--; // Version numarasını geri al
    }

    /// <summary>
    /// İki aggregate'in aynı olup olmadığını kontrol eder.
    /// ID ve Version kontrolü yapar.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not BaseAggregateRoot<TKey> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id) && Version == other.Version;
    }

    /// <summary>
    /// Aggregate için unique bir hash code üretir.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Version);
    }
}