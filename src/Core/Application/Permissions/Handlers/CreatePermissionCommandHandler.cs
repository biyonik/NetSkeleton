using Application.Common.Results;
using Application.Permissions.Commands;
using Application.Permissions.Dtos;
using AutoMapper;
using Domain.Authorization;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class CreatePermissionCommandHandler(
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<CreatePermissionCommand, Result<PermissionDto>>
{
    public async Task<Result<PermissionDto>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        // System name unique kontrolü
        if (await authorizationRepository.PermissionExistsAsync(request.SystemName, null, cancellationToken))
            return Result.Failure<PermissionDto>("Permission with this system name already exists");

        var permission = Permission.Create(
            request.Name,
            request.SystemName,
            request.Description,
            request.Category,
            request.IsSystemPermission);

        if (!string.IsNullOrEmpty(request.RequiredClaims))
            permission.SetRequiredClaims(request.RequiredClaims);

        await authorizationRepository.AddPermissionAsync(permission, cancellationToken);

        return Result.Success(mapper.Map<PermissionDto>(permission));
    }
}