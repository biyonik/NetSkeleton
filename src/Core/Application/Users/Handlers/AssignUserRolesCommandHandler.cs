using Application.Common.Results;
using Application.Users.Commands;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Handlers;

public class AssignUserRolesCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<AssignUserRolesCommand, Result>
{
    public async Task<Result> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Result.Failure(Error.NotFound);

        var currentRoles = await userManager.GetRolesAsync(user);
        
        // Rolleri kaldır
        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return Result.Failure(string.Join(", ", removeResult.Errors.Select(e => e.Description)));

        // Yeni rolleri ekle
        var addResult = await userManager.AddToRolesAsync(user, request.Roles);
        return !addResult.Succeeded 
            ? Result.Failure(string.Join(", ", addResult.Errors.Select(e => e.Description))) 
            : Result.Success();
    }
}