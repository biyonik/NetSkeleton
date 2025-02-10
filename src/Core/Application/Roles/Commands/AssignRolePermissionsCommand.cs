using Application.Common.Results;
using MediatR;

namespace Application.Roles.Commands;

public record AssignRolePermissionsCommand : IRequest<Result>
{
    public Guid RoleId { get; init; }
    public List<string> Permissions { get; init; } = new();
}