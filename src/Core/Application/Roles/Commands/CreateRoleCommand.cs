using Application.Common.Results;
using Application.Roles.Dtos;
using Domain.Authorization;
using MediatR;

namespace Application.Roles.Commands;

public record CreateRoleCommand : IRequest<Result<RoleDto>>
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public List<string> Permissions { get; init; } = new();
}