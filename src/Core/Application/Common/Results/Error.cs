namespace Application.Common.Results;

/// <summary>
/// Error sınıfı
/// </summary>
public record Error(string Code, string Message)
{
    public Dictionary<string, List<string>> ValidationErrors { get; private set; }

    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");
    public static readonly Error NotFound = new("Error.NotFound", "Resource not found");
    public static readonly Error Unauthorized = new("Error.Unauthorized", "Unauthorized access");
    public static readonly Error Forbidden = new("Error.Forbidden", "Forbidden access");
    public static readonly Error Conflict = new("Error.Conflict", "Resource conflict");

    public static Error Custom(string message) => new("Error.Custom", message);

    public static Error Validation(Dictionary<string, List<string>> validationErrors)
    {
        return new Error("Error.Validation", "Validasyon hataları oluştu")
        {
            ValidationErrors = validationErrors
        };
    }
}