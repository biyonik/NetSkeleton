namespace Infrastructure.Email.Exceptions;

/// <summary>
/// Email ile ilgili özel exception'lar
/// </summary>
public class EmailException : Exception
{
    public EmailException(string message) : base(message) { }
    public EmailException(string message, Exception inner) : base(message, inner) { }
}