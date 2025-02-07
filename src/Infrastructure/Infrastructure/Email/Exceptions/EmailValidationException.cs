namespace Infrastructure.Email.Exceptions;

/// <summary>
/// Email ile ilgili özel exception'lar
/// </summary>
public class EmailValidationException(string message) : EmailException(message);