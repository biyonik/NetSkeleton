namespace Application.Common.Results;

/// <summary>
/// Validation sonuçları için result sınıfı
/// </summary>
public class ValidationResult : Result
{
    public IReadOnlyList<ValidationError> Errors { get; }

    private ValidationResult(IReadOnlyList<ValidationError> errors)
        : base(false, Error.Validation, "Validation failed")
    {
        Errors = errors;
    }

    public static ValidationResult WithErrors(IReadOnlyList<ValidationError> errors) => new(errors);
}
