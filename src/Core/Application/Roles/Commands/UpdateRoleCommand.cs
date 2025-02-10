using Application.Common.Results;
using Application.Roles.Dtos;
using MediatR;

namespace Application.Roles.Commands;

public record UpdateRoleCommand : IRequest<Result<RoleDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}