using Application.Auth.Commands;
using Application.Common.Results;
using Application.Common.Security.Services;
using MediatR;

namespace Application.Auth.Handlers;

public class RevokeTokenCommandHandler(IJwtService jwtService) : IRequestHandler<RevokeTokenCommand, Result>
{
    public async Task<Result> Handle(
        RevokeTokenCommand request, 
        CancellationToken cancellationToken)
    {
        await jwtService.RevokeTokenAsync(request.UserId);
        return Result.Success();
    }
}