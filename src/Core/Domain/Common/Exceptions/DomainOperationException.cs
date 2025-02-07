namespace Domain.Common.Exceptions;

/// <summary>
/// Domain operasyonlarının izin verilmeyen durumlarında fırlatılan exception
/// </summary>
public class DomainOperationException(string operation, string reason)
    : DomainException($"Domain operation '{operation}' failed: {reason}")
{
    /// <summary>
    /// Operasyon adı
    /// </summary>
    public string Operation { get; } = operation;

    /// <summary>
    /// İzin verilmeyen durumun nedeni
    /// </summary>
    public string Reason { get; } = reason;
}