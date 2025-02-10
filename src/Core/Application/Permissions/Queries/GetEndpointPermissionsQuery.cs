using Application.Common.Results;
using Application.Permissions.Dtos;
using MediatR;

namespace Application.Permissions.Queries;

public record GetEndpointPermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    public string Controller { get; init; } = null!;
    public string Action { get; init; } = null!;
    public string HttpMethod { get; init; } = null!;
}