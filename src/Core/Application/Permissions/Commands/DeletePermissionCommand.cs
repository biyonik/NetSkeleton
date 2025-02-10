using Application.Common.Results;
using MediatR;

namespace Application.Permissions.Commands;

public record DeletePermissionCommand(Guid Id) : IRequest<Result>;
