using Application.Common.Results;
using MediatR;

namespace Application.Common.CQRS;

/// <summary>
/// Command handler'lar için base interface
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Void dönüş tipli command handler'lar için
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}