using Infrastructure.Email.Abstractions;
using Infrastructure.Email.Settings;
using Infrastructure.Email.Strategies;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email.Factory;

/// <summary>
/// Email factory implementasyonu
/// </summary>
public class EmailStrategyFactory(IOptions<EmailSettings> settings) : IEmailStrategyFactory
{
    /// <summary>
    /// Verilen strateji tipine göre email implementasyonu oluşturur
    /// </summary>
    public IEmailStrategy CreateStrategy(EmailStrategy strategy)
    {
        return strategy switch
        {
            EmailStrategy.Smtp => new SmtpEmailStrategy(settings),
            EmailStrategy.File => new FileEmailStrategy(settings),
            EmailStrategy.SendGrid => throw new NotImplementedException("SendGrid strategy is not implemented yet."),
            _ => throw new ArgumentException($"Unsupported email strategy: {strategy}")
        };
    }
}