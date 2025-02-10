using Domain.Common.Abstractions;
using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Access Rule value object'i
/// İzin kısıtlamalarını temsil eder
/// </summary>
public class AccessRule : BaseValueObject
{
    public string Type { get; }
    public string Value { get; }
    public Dictionary<string, string> Parameters { get; }

    private AccessRule(string type, string value, Dictionary<string, string>? parameters = null)
    {
        Type = type;
        Value = value;
        Parameters = parameters ?? new Dictionary<string, string>();
    }

    public static AccessRule Create(string type, string value, Dictionary<string, string>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Access rule type cannot be empty");

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Access rule value cannot be empty");

        return new AccessRule(type, value, parameters);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return Value;
        foreach (var parameter in Parameters.OrderBy(x => x.Key))
        {
            yield return parameter.Key;
            yield return parameter.Value;
        }
    }
}
