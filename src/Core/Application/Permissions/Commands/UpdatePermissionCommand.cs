using Application.Common.Results;
using Application.Permissions.Dtos;
using MediatR;

namespace Application.Permissions.Commands;

public record UpdatePermissionCommand : IRequest<Result<PermissionDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? Category { get; init; }
    public string? RequiredClaims { get; init; }
    public bool IsActive { get; init; }
}