using Application.Common.Results;
using Application.Roles.Commands;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Handlers;

public class DeleteRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    IAuthorizationRepository authorizationRepository)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IAuthorizationRepository _authorizationRepository = authorizationRepository;

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
            return Result.Success(); // Role zaten yok

        // SuperAdmin rolü silinemez
        if (role.Name == "SuperAdmin")
            return Result.Failure("Cannot delete SuperAdmin role");

        var deleteResult = await roleManager.DeleteAsync(role);
        if (!deleteResult.Succeeded)
            return Result.Failure(string.Join(", ", deleteResult.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}