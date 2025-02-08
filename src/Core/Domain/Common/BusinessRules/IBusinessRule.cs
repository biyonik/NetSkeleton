namespace Domain.Common.BusinessRules;

/// <summary>
/// Business rule'lar için interface
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Kural adı
    /// </summary>
    string RuleName { get; }

    /// <summary>
    /// Kural detayı
    /// </summary>
    string Details { get; }

    /// <summary>
    /// Kuralın geçerli olup olmadığını kontrol eder
    /// </summary>
    Task<bool> IsValid();
}