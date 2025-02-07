using Infrastructure.Email.Abstractions;
using Infrastructure.Email.Settings;

namespace Infrastructure.Email.Factory;

/// <summary>
/// Email stratejisi oluşturmak için factory interface
/// </summary>
public interface IEmailStrategyFactory
{
    /// <summary>
    /// İstenilen email stratejisini oluşturur
    /// </summary>
    IEmailStrategy CreateStrategy(EmailStrategy strategy);
}