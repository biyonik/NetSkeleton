using Application.Common.Results;
using Application.Roles.Commands;
using Domain.Authorization;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Handlers;

public class AssignRolePermissionsCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IAuthorizationRepository authorizationRepository)
    : IRequestHandler<AssignRolePermissionsCommand, Result>
{
    public async Task<Result> Handle(AssignRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role == null)
            return Result.Failure(Error.NotFound);

        // Mevcut permission'ları temizle
        var currentGrants = await authorizationRepository
            .GetPermissionGrantsAsync(request.RoleId, true, cancellationToken);

        foreach (var grant in currentGrants)
        {
            await authorizationRepository.DeleteGrantAsync(grant, cancellationToken);
        }

        // Yeni permission'ları ekle
        foreach (var permission in request.Permissions)
        {
            var grant = PermissionGrant.Create(role.Id.ToString(), Guid.NewGuid(), permission);
            await authorizationRepository.AddGrantAsync(grant, cancellationToken);
        }

        return Result.Success();
    }
}