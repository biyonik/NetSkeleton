using Application.Common.Results;
using Application.Users.Commands;
using Domain.Authorization;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Users.Handlers;

public class AssignUserPermissionsCommandHandler(IAuthorizationRepository authorizationRepository)
    : IRequestHandler<AssignUserPermissionsCommand, Result>
{
    public async Task<Result> Handle(AssignUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        // Mevcut permission'ları deaktive et
        var currentGrants = await authorizationRepository.GetUserGrantsAsync(
            request.UserId.ToString(), true, cancellationToken);

        foreach (var grant in currentGrants)
        {
            await authorizationRepository.DeleteGrantAsync(grant, cancellationToken);
        }

        // Yeni permission'ları ekle
        foreach (var grant in request.Permissions.Select(permission => PermissionGrant.Create(
                     request.UserId.ToString(),
                     Guid.NewGuid(),
                     permission)))
        {
            await authorizationRepository.AddGrantAsync(grant, cancellationToken);
        }

        return Result.Success();
    }
}