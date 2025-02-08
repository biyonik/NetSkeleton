namespace Application.Common.Results;

/// <summary>
/// Validation hatası sınıfı
/// </summary>
public record ValidationError(string PropertyName, string Message);