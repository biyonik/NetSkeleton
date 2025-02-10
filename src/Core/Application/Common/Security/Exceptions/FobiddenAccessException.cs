namespace Application.Common.Security.Exceptions;

/// <summary>
/// Yetkilendirme hataları için özel exception'lar
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Access forbidden")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }
}