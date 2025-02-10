using Application.Common.Results;
using MediatR;

namespace Application.Users.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;
