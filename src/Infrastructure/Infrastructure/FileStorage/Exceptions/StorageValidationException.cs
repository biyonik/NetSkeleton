namespace Infrastructure.FileStorage.Exceptions;

/// <summary>
/// Storage validasyon hatalarını temsil eden exception
/// </summary>
public class StorageValidationException(string message) : StorageException(message);
