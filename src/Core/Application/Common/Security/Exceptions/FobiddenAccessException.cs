namespace Application.Common.Security.Exceptions;

/// <summary>
/// Yetkilendirme hataları için özel exception'lar
/// </summary>
public class ForbiddenAccessException() : Exception("Access forbidden");