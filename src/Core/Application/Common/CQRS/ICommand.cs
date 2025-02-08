using Application.Common.Results;
using MediatR;

namespace Application.Common.CQRS;

/// <summary>
/// Tüm Command'lar için marker interface
/// </summary>
public interface ICommand<TResponse>: IRequest<Result<TResponse>>
{
}

/// <summary>
/// Void dönüş tipli Command'lar için
/// </summary>
public interface ICommand: IRequest<Result>
{
}