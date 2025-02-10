using Application.Common.Results;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries;

public record GetUserQuery(Guid Id) : IRequest<Result<UserDto>>;

