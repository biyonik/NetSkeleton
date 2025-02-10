using Application.Common.Results;
using Application.Permissions.Dtos;
using Application.Permissions.Queries;
using AutoMapper;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class GetEndpointPermissionsQueryHandler(
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetEndpointPermissionsQuery, Result<List<PermissionDto>>>
{
    public async Task<Result<List<PermissionDto>>> Handle(
        GetEndpointPermissionsQuery request, 
        CancellationToken cancellationToken)
    {
        var permissions = await authorizationRepository.GetPermissionsForEndpointAsync(
            request.Controller,
            request.Action,
            request.HttpMethod,
            cancellationToken);

        return Result.Success(mapper.Map<List<PermissionDto>>(permissions));
    }
}