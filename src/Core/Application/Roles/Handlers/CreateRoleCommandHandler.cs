using Application.Common.Results;
using Application.Roles.Commands;
using Application.Roles.Dtos;
using AutoMapper;
using Domain.Authorization;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Handlers;

public class CreateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // Role adı unique mi kontrol et
        if (await roleManager.RoleExistsAsync(request.Name))
            return Result.Failure<RoleDto>("Role with this name already exists");

        var role = new ApplicationRole
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            CreatedDate = DateTime.UtcNow
        };

        var createResult = await roleManager.CreateAsync(role);
        if (!createResult.Succeeded)
            return Result.Failure<RoleDto>(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        // Permission'ları ekle
        foreach (var permission in request.Permissions)
        {
            var grant = PermissionGrant.Create(role.Id.ToString(), Guid.NewGuid(), permission);
            await authorizationRepository.AddGrantAsync(grant, cancellationToken);
        }

        var roleDto = mapper.Map<RoleDto>(role);
        roleDto.Permissions = request.Permissions;

        return Result.Success(roleDto);
    }
}