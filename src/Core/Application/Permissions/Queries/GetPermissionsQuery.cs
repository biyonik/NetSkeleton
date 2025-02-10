using Application.Common.Results;
using Application.Permissions.Dtos;
using MediatR;

namespace Application.Permissions.Queries;

public record GetPermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    public string? SearchTerm { get; init; }
    public string? Category { get; init; }
    public bool IncludeInactive { get; init; } = false;
    public int? Skip { get; init; }
    public int? Take { get; init; }
}