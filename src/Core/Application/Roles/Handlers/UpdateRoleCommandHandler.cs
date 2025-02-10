using Application.Common.Results;
using Application.Roles.Commands;
using Application.Roles.Dtos;
using AutoMapper;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Handlers;

public class UpdateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
            return Result.Failure<RoleDto>(Error.NotFound);

        // Diğer aynı isimli rol var mı kontrol et
        var existingRole = await roleManager.FindByNameAsync(request.Name);
        if (existingRole != null && existingRole.Id != request.Id)
            return Result.Failure<RoleDto>("Role with this name already exists");

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsActive = request.IsActive;
        role.LastModifiedDate = DateTime.UtcNow;

        var updateResult = await roleManager.UpdateAsync(role);
        if (!updateResult.Succeeded)
            return Result.Failure<RoleDto>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        var roleDto = mapper.Map<RoleDto>(role);
        roleDto.Permissions = await authorizationRepository
            .GetUserPermissionSystemNamesAsync(role.Id.ToString(), cancellationToken);

        return Result.Success(roleDto);
    }
}