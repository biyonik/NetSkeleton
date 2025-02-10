using Application.Common.Results;
using Application.Permissions.Dtos;
using Application.Permissions.Queries;
using AutoMapper;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class GetPermissionQueryHandler(
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetPermissionQuery, Result<PermissionDto>>
{
    public async Task<Result<PermissionDto>> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
    {
        var permission = await authorizationRepository.GetPermissionByIdAsync(request.Id, cancellationToken);
        if (permission == null)
            return Result.Failure<PermissionDto>(Error.NotFound);

        return Result.Success(mapper.Map<PermissionDto>(permission));
    }
}
