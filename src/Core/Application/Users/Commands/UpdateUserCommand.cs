using Application.Common.Results;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Commands;

public record UpdateUserCommand : IRequest<Result<UserDto>>
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public bool IsActive { get; init; }
}