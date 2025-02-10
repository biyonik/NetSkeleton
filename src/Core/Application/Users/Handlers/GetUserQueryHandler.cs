using Application.Common.Results;
using Application.Users.Dtos;
using Application.Users.Queries;
using AutoMapper;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Handlers;

public class GetUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Result.Failure<UserDto>(Error.NotFound);

        var userDto = mapper.Map<UserDto>(user);
        userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();
        userDto.Permissions = await authorizationRepository
            .GetUserPermissionSystemNamesAsync(user.Id.ToString(), cancellationToken);

        return Result.Success(userDto);
    }
}