using Domain.Common.Exceptions;

namespace Domain.Common.Guards;

/// <summary>
/// Domain kurallarını korumak için kullanılan Guard yapısı.
/// Metodların başında validation kontrollerini yapmak için kullanılır.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Bir değerin null olup olmadığını kontrol eder
    /// </summary>
    public static void AgainstNull<T>(T value, string parameterName)
    {
        if (value == null)
        {
            throw new BusinessRuleValidationException(
                "NullValueNotAllowed", 
                $"Parameter {parameterName} cannot be null");
        }
    }

    /// <summary>
    /// String bir değerin null veya boş olup olmadığını kontrol eder
    /// </summary>
    public static void AgainstNullOrEmpty(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException(
                "EmptyStringNotAllowed", 
                $"Parameter {parameterName} cannot be null or empty");
        }
    }

    /// <summary>
    /// Bir değerin negatif olup olmadığını kontrol eder
    /// </summary>
    public static void AgainstNegative(decimal value, string parameterName)
    {
        if (value < 0)
        {
            throw new BusinessRuleValidationException(
                "NegativeValueNotAllowed", 
                $"Parameter {parameterName} cannot be negative. Current value: {value}");
        }
    }

    /// <summary>
    /// Bir değerin belirtilen aralıkta olup olmadığını kontrol eder
    /// </summary>
    public static void AgainstOutOfRange<T>(T value, T min, T max, string parameterName) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new BusinessRuleValidationException(
                "ValueOutOfRange", 
                $"Parameter {parameterName} must be between {min} and {max}. Current value: {value}");
        }
    }

    /// <summary>
    /// Bir koleksiyonun boş olup olmadığını kontrol eder
    /// </summary>
    public static void AgainstEmptyCollection<T>(IEnumerable<T> collection, string parameterName)
    {
        if (!collection.Any())
        {
            throw new BusinessRuleValidationException(
                "EmptyCollectionNotAllowed", 
                $"Collection {parameterName} cannot be empty");
        }
    }

    /// <summary>
    /// Bir koleksiyonun maksimum boyutunu kontrol eder
    /// </summary>
    public static void AgainstExceedingLength<T>(IEnumerable<T> collection, int maxLength, string parameterName)
    {
        if (collection.Count() > maxLength)
        {
            throw new BusinessRuleValidationException(
                "CollectionTooLarge", 
                $"Collection {parameterName} cannot have more than {maxLength} items");
        }
    }

    /// <summary>
    /// Bir string'in maksimum uzunluğunu kontrol eder
    /// </summary>
    public static void AgainstExceedingLength(string value, int maxLength, string parameterName)
    {
        if (value.Length > maxLength)
        {
            throw new BusinessRuleValidationException(
                "StringTooLong", 
                $"String {parameterName} cannot be longer than {maxLength} characters");
        }
    }

    /// <summary>
    /// Default/boş Guid kontrolü yapar
    /// </summary>
    public static void AgainstDefaultGuid(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new BusinessRuleValidationException(
                "EmptyGuidNotAllowed", 
                $"Parameter {parameterName} cannot be an empty GUID");
        }
    }

    /// <summary>
    /// İki tarih arasındaki ilişkiyi kontrol eder
    /// </summary>
    public static void AgainstInvalidDateRange(DateTime startDate, DateTime endDate, string parameterName)
    {
        if (endDate < startDate)
        {
            throw new BusinessRuleValidationException(
                "InvalidDateRange", 
                $"End date cannot be earlier than start date in {parameterName}");
        }
    }

    /// <summary>
    /// Email formatını kontrol eder
    /// </summary>
    public static void AgainstInvalidEmail(string email, string parameterName)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
            {
                throw new BusinessRuleValidationException(
                    "InvalidEmailFormat", 
                    $"Parameter {parameterName} is not a valid email address");
            }
        }
        catch
        {
            throw new BusinessRuleValidationException(
                "InvalidEmailFormat", 
                $"Parameter {parameterName} is not a valid email address");
        }
    }

    /// <summary>
    /// Özel bir business rule'u kontrol eder
    /// </summary>
    public static void AgainstBusinessRule(Func<bool> rule, string ruleName, string details)
    {
        if (!rule())
        {
            throw new BusinessRuleValidationException(ruleName, details);
        }
    }
}