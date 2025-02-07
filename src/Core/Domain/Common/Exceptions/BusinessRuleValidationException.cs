namespace Domain.Common.Exceptions;

/// <summary>
/// Business rule ihlallerinde fırlatılan exception
/// </summary>
public class BusinessRuleValidationException(string brokenRule, string details)
    : DomainException($"Business rule '{brokenRule}' broken: {details}")
{
    /// <summary>
    /// İhlal edilen kuralın adı
    /// </summary>
    public string BrokenRule { get; } = brokenRule;

    /// <summary>
    /// Kural detayları
    /// </summary>
    public string Details { get; } = details;
}