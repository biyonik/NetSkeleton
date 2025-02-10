using Application.Common.Results;
using MediatR;

namespace Application.Auth.Commands;

public record RevokeTokenCommand(string UserId) : IRequest<Result>;
