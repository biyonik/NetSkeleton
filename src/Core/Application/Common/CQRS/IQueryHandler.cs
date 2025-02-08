using Application.Common.Results;
using MediatR;

namespace Application.Common.CQRS;

/// <summary>
/// Query handler'lar için base interface
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>, IRequest<Result<TResponse>>
{
}