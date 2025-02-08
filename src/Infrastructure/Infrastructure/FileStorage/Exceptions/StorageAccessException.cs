namespace Infrastructure.FileStorage.Exceptions;

/// <summary>
/// Storage'a erişim hatalarını temsil eden exception
/// </summary>
public class StorageAccessException : StorageException
{
    public StorageAccessException(string message) : base(message)
    {
    }

    public StorageAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}