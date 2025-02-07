namespace Domain.Common.Abstractions;

/// <summary>
/// Aggregate Root tasarım desenini işaretlemek için kullanılan interface.
/// Bu interface'i implemente eden sınıflar bir Aggregate Root olarak kabul edilir.
/// 
/// Aggregate Root:
/// - Bir transaction boundary'sini temsil eder
/// - İlişkili entity'lerin tutarlılığını sağlar
/// - Domain kurallarını ve invariant'ları uygular
/// - Repository pattern'de doğrudan erişilebilen tek entity'dir
/// </summary>
/// <typeparam name="TKey">Aggregate root'un primary key tipi</typeparam>
public interface IAggregateRoot<TKey> : IEntity<TKey> where TKey : struct
{
    /// <summary>
    /// Aggregate'in versiyon numarası.
    /// Optimistic concurrency için kullanılır.
    /// Her güncelleme işleminde otomatik olarak artar.
    /// </summary>
    int Version { get; }
    
    /// <summary>
    /// Aggregate üzerinde bekleyen değişikliklerin sayısı.
    /// Unit of Work pattern'de kullanılır.
    /// </summary>
    int PendingChanges { get; }
}