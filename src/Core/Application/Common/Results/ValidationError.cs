namespace Application.Common.Results;

/// <summary>
/// Validation hatası sınıfı
/// </summary>
public partial record ValidationError()
{
    public Dictionary<string, List<string>> Errors { get; init; } = new();
    
    public ValidationError(string key, string message) : this()
    {
        Errors.Add(key, new List<string> { message });
    }
    
    public ValidationError(Dictionary<string, List<string>> errors) : this()
    {
        Errors = errors;
    }
}


