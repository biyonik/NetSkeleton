using Application.Common.Results;
using Application.Permissions.Commands;
using Application.Permissions.Dtos;
using AutoMapper;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class UpdatePermissionCommandHandler(
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<UpdatePermissionCommand, Result<PermissionDto>>
{
    public async Task<Result<PermissionDto>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await authorizationRepository.GetPermissionByIdAsync(request.Id, cancellationToken);
        if (permission == null)
            return Result.Failure<PermissionDto>(Error.NotFound);

        if (permission.IsSystemPermission)
            return Result.Failure<PermissionDto>("System permissions cannot be modified");

        permission.Update(request.Name, request.Description, request.Category);
        
        if (!string.IsNullOrEmpty(request.RequiredClaims))
            permission.SetRequiredClaims(request.RequiredClaims);

        await authorizationRepository.UpdatePermissionAsync(permission, cancellationToken);

        return Result.Success(mapper.Map<PermissionDto>(permission));
    }
}