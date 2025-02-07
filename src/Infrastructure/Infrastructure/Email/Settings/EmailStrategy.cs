namespace Infrastructure.Email.Settings;

/// <summary>
/// Email stratejileri
/// </summary>
public enum EmailStrategy
{
    Smtp = 1,
    SendGrid = 2,
    File = 3
}