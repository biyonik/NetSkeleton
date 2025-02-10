using Application.Common.Results;
using MediatR;

namespace Application.Roles.Commands;

public record DeleteRoleCommand(Guid Id) : IRequest<Result>;
