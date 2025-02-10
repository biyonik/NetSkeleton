using Application.Common.Results;
using Application.Common.Security.Response;
using MediatR;

namespace Application.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<TokenResponse>>;
