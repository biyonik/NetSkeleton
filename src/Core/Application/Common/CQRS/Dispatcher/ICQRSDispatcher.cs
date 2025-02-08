using Application.Common.Results;

namespace Application.Common.CQRS.Dispatcher;

/// <summary>
/// CQRS yönetimi için temel servis
/// </summary>
public interface ICQRSDispatcher
{
    Task<Result<TResponse>> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
}