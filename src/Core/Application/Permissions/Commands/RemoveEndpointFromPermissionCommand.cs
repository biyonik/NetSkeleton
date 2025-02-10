using Application.Common.Results;
using MediatR;

namespace Application.Permissions.Commands;

public record RemoveEndpointFromPermissionCommand(Guid EndpointId) : IRequest<Result>;
