namespace Domain.Common.Abstractions;

/// <summary>
/// Tüm entity'ler için temel kontrat. 
/// Bu interface ile bir sınıfın entity olduğunu işaretliyoruz.
/// </summary>
/// <typeparam name="TKey">Entity'nin primary key tipi</typeparam>
public interface IEntity<TKey> where TKey : struct
{
    /// <summary>
    /// Entity'nin benzersiz tanımlayıcısı
    /// </summary>
    TKey Id { get; }
    
    /// <summary>
    /// Entity'nin aktif/pasif durumu
    /// Soft delete pattern için kullanılır
    /// </summary>
    bool IsActive { get; }
    
    /// <summary>
    /// Entity'nin oluşturulma tarihi
    /// </summary>
    DateTime CreatedDate { get; }
    
    /// <summary>
    /// Entity'yi oluşturan kullanıcı ID'si
    /// </summary>
    string? CreatedBy { get; }
    
    /// <summary>
    /// Entity'nin son güncelleme tarihi
    /// </summary>
    DateTime? LastModifiedDate { get; }
    
    /// <summary>
    /// Entity'yi son güncelleyen kullanıcı ID'si
    /// </summary>
    string? LastModifiedBy { get; }
}