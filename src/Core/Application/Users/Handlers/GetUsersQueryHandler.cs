using Application.Common.Results;
using Application.Users.Dtos;
using Application.Users.Queries;
using AutoMapper;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Handlers;

public class GetUsersQueryHandler(
    UserManager<ApplicationUser> userManager,
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(u => 
                u.Email.ToLower().Contains(searchTerm) ||
                u.FirstName.ToLower().Contains(searchTerm) ||
                u.LastName.ToLower().Contains(searchTerm));
        }

        if (!request.IncludeInactive)
        {
            query = query.Where(u => u.IsActive);
        }

        if (request.Skip.HasValue && request.Take.HasValue)
        {
            query = query.Skip(request.Skip.Value).Take(request.Take.Value);
        }

        var users = await query.ToListAsync(cancellationToken);
        var userDtos = mapper.Map<List<UserDto>>(users);

        // Her kullanıcı için rol ve permission'ları yükle
        foreach (var userDto in userDtos)
        {
            var user = users.First(u => u.Id == userDto.Id);
            userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();
            userDto.Permissions = await authorizationRepository
                .GetUserPermissionSystemNamesAsync(user.Id.ToString(), cancellationToken);
        }

        return Result.Success(userDtos);
    }
}