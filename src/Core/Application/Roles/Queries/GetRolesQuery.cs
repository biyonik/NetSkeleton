using Application.Common.Results;
using Application.Roles.Dtos;
using MediatR;

namespace Application.Roles.Queries;

public record GetRolesQuery : IRequest<Result<List<RoleDto>>>
{
    public string? SearchTerm { get; init; }
    public bool IncludeInactive { get; init; } = false;
    public int? Skip { get; init; }
    public int? Take { get; init; }
}