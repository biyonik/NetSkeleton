using Application.Common.Results;
using Application.Roles.Dtos;
using Application.Roles.Queries;
using AutoMapper;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Handlers;

public class GetRolesQueryHandler(
    RoleManager<ApplicationRole> roleManager,
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var query = roleManager.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(r => 
                r.Name.ToLower().Contains(searchTerm) ||
                (r.Description != null && r.Description.ToLower().Contains(searchTerm)));
        }

        if (!request.IncludeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        if (request.Skip.HasValue && request.Take.HasValue)
        {
            query = query.Skip(request.Skip.Value).Take(request.Take.Value);
        }

        var roles = await query.ToListAsync(cancellationToken);
        var roleDtos = mapper.Map<List<RoleDto>>(roles);

        // Her rol için permission'ları yükle
        foreach (var roleDto in roleDtos)
        {
            roleDto.Permissions = await authorizationRepository
                .GetUserPermissionSystemNamesAsync(roleDto.Id.ToString(), cancellationToken);
        }

        return Result.Success(roleDtos);
    }
}