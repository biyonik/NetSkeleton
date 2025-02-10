using Application.Common.Results;
using MediatR;

namespace Application.Permissions.Commands;

public record AssignEndpointToPermissionCommand : IRequest<Result>
{
    public Guid PermissionId { get; init; }
    public string Controller { get; init; } = null!;
    public string Action { get; init; } = null!;
    public string HttpMethod { get; init; } = null!;
    public string Route { get; init; } = null!;
}