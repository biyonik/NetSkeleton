using Application.Common.Results;
using Application.Users.Commands;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Handlers;

public class DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Result.Success(); // Kullanıcı zaten yok

        var result = await userManager.DeleteAsync(user);
        return !result.Succeeded 
            ? Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description))) 
            : Result.Success();
    }
}