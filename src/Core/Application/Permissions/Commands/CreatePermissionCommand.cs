using Application.Common.Results;
using Application.Permissions.Dtos;
using MediatR;

namespace Application.Permissions.Commands;

public record CreatePermissionCommand : IRequest<Result<PermissionDto>>
{
    public string Name { get; init; } = null!;
    public string SystemName { get; init; } = null!;
    public string? Description { get; init; }
    public string? Category { get; init; }
    public bool IsSystemPermission { get; init; }
    public string? RequiredClaims { get; init; }
}