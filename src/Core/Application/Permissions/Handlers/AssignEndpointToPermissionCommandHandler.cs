using Application.Common.Results;
using Application.Permissions.Commands;
using Domain.Authorization;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class AssignEndpointToPermissionCommandHandler(IAuthorizationRepository authorizationRepository)
    : IRequestHandler<AssignEndpointToPermissionCommand, Result>
{
    public async Task<Result> Handle(
        AssignEndpointToPermissionCommand request, 
        CancellationToken cancellationToken)
    {
        var permission = await authorizationRepository
            .GetPermissionByIdAsync(request.PermissionId, cancellationToken);

        if (permission == null)
            return Result.Failure(Error.NotFound);

        var endpoint = PermissionEndpoint.Create(
            request.PermissionId,
            request.Controller,
            request.Action,
            request.HttpMethod,
            request.Route);

        await authorizationRepository
            .AddEndpointToPermissionAsync(endpoint, cancellationToken);

        return Result.Success();
    }
}