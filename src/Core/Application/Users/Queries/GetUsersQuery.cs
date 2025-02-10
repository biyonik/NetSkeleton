using Application.Common.Results;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries;

public record GetUsersQuery : IRequest<Result<List<UserDto>>>
{
    public string? SearchTerm { get; init; }
    public bool IncludeInactive { get; init; } = false;
    public int? Skip { get; init; }
    public int? Take { get; init; }
}