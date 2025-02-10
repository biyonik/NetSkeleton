using Application.Common.Results;
using Application.Permissions.Dtos;
using Application.Permissions.Queries;
using AutoMapper;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class GetPermissionEndpointsQueryHandler(
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetPermissionEndpointsQuery, Result<List<PermissionEndpointDto>>>
{
    public async Task<Result<List<PermissionEndpointDto>>> Handle(
        GetPermissionEndpointsQuery request, 
        CancellationToken cancellationToken)
    {
        var endpoints = await authorizationRepository
            .GetEndpointsForPermissionAsync(request.PermissionId, cancellationToken);

        return Result.Success(mapper.Map<List<PermissionEndpointDto>>(endpoints));
    }
}