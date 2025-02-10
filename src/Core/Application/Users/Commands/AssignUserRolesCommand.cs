using Application.Common.Results;
using MediatR;

namespace Application.Users.Commands;

public record AssignUserRolesCommand : IRequest<Result>
{
    public Guid UserId { get; init; }
    public List<string> Roles { get; init; } = new();
}
