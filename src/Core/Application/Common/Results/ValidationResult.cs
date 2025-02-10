namespace Application.Common.Results;

/// <summary>
/// Validation sonuçları için result sınıfı
/// </summary>
public class ValidationResult : Result
{
    public Dictionary<string, List<string>> Errors { get; }

    internal ValidationResult(Dictionary<string, List<string>> errors)
        : base(false, Error.Validation(errors), "Validation failed")
    {
        Errors = errors;
    }

    public static ValidationResult WithErrors(Dictionary<string, List<string>> errors) => new(errors);
}
