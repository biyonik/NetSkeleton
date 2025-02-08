using Application.Common.Results;
using MediatR;

namespace Application.Common.CQRS;

/// <summary>
/// Tüm Query'ler için marker interface
/// </summary>
public interface IQuery<TResponse>: IRequest<Result<TResponse>>
{
}