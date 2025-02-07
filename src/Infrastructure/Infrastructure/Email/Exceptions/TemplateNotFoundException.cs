namespace Infrastructure.Email.Exceptions;

/// <summary>
/// Template bulunamadığında fırlatılan exception
/// </summary>
public class TemplateNotFoundException(string message) : Exception(message);