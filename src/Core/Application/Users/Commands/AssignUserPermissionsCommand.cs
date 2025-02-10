using Application.Common.Results;
using MediatR;

namespace Application.Users.Commands;

public record AssignUserPermissionsCommand : IRequest<Result>
{
    public Guid UserId { get; init; }
    public List<string> Permissions { get; init; } = new();
}