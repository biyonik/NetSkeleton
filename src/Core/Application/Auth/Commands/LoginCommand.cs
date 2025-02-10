using Application.Common.Results;
using Application.Common.Security.Response;
using MediatR;

namespace Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<TokenResponse>>;
