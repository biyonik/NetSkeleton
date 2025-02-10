using Application.Common.Results;
using Application.Roles.Dtos;
using Application.Roles.Queries;
using AutoMapper;
using Domain.Authorization;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Handlers;

public class GetRoleQueryHandler(
    RoleManager<ApplicationRole> roleManager,
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetRoleQuery, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
            return Result.Failure<RoleDto>(Error.NotFound);

        var roleDto = mapper.Map<RoleDto>(role);
        var permissions = await authorizationRepository
            .GetUserPermissionSystemNamesAsync(role.Id.ToString(), cancellationToken);

        foreach (var permission in permissions)
        {
            // roleDto.Permissions.Add(Permission.Create(
            //     permission,
            //     "Permission",
            //     "Permission",
            // ));
        }

        return Result.Success(roleDto);
    }
}