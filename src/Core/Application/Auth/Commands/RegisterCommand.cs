using Application.Common.Results;
using MediatR;

namespace Application.Auth.Commands;

public record RegisterCommand(string Email, string FirstName, string LastName, string Password) : IRequest<Result>;