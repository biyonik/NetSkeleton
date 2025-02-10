using Application.Common.Results;
using Application.Permissions.Commands;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class DeletePermissionCommandHandler(IAuthorizationRepository authorizationRepository)
    : IRequestHandler<DeletePermissionCommand, Result>
{
    public async Task<Result> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await authorizationRepository.GetPermissionByIdAsync(request.Id, cancellationToken);
        if (permission == null)
            return Result.Success(); // Permission zaten yok

        if (permission.IsSystemPermission)
            return Result.Failure("System permissions cannot be deleted");

        await authorizationRepository.DeletePermissionAsync(permission, cancellationToken);
        return Result.Success();
    }
}