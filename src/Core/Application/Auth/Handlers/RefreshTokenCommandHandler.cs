using Application.Auth.Commands;
using Application.Common.Exceptions;
using Application.Common.Results;
using Application.Common.Security.Response;
using Application.Common.Security.Services;
using MediatR;

namespace Application.Auth.Handlers;

public class RefreshTokenCommandHandler(IJwtService jwtService)
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(
        RefreshTokenCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await jwtService.RefreshTokenAsync(
                request.AccessToken, 
                request.RefreshToken);
            
            return Result.Success(token);
        }
        catch (SecurityTokenException ex)
        {
            return Result.Failure<TokenResponse>(Error.Unauthorized);
        }
    }
}