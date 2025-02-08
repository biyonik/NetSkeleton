using Application.Common.Results;
using MediatR;

namespace Application.Common.CQRS.Dispatcher;

/// <summary>
/// CQRS dispatcher implementasyonu
/// </summary>
public class CQRSDispatcher(IMediator mediator) : ICQRSDispatcher
{
    public Task<Result<TResponse>> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        return mediator.Send<Result<TResponse>>(query, cancellationToken);
    }

    public Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        return mediator.Send(command, cancellationToken);
    }

    public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        return mediator.Send(command, cancellationToken);
    }
}