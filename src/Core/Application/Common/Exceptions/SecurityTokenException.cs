namespace Application.Common.Exceptions;

public class SecurityTokenException : Exception
{
    public SecurityTokenException() : base("Invalid security token")
    {
    }

    public SecurityTokenException(string message) : base(message)
    {
    }

    public SecurityTokenException(string message, Exception inner) : base(message, inner)
    {
    }
}