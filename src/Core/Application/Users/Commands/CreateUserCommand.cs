using Application.Common.Results;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public List<string> Roles { get; init; } = new();
    public List<string> Permissions { get; init; } = new();
}
