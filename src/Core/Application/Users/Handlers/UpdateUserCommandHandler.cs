using Application.Common.Results;
using Application.Users.Commands;
using Application.Users.Dtos;
using AutoMapper;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Handlers;

public class UpdateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper)
    : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Result.Failure<UserDto>(Error.NotFound);

        user.Email = request.Email;
        user.UserName = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.IsActive = request.IsActive;
        user.LastModifiedDate = DateTime.UtcNow;

        var result = await userManager.UpdateAsync(user);
        return !result.Succeeded 
            ? Result.Failure<UserDto>(string.Join(", ", result.Errors.Select(e => e.Description))) 
            : Result.Success(mapper.Map<UserDto>(user));
    }
}