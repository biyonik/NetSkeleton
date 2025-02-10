using Application.Common.Results;
using Application.Permissions.Dtos;
using Application.Permissions.Queries;
using AutoMapper;
using Domain.Authorization.Repositories;
using Domain.Authorization.Specifications;
using MediatR;

namespace Application.Permissions.Handlers;

public class GetPermissionsQueryHandler(
    IAuthorizationRepository authorizationRepository,
    IMapper mapper)
    : IRequestHandler<GetPermissionsQuery, Result<List<PermissionDto>>>
{
    public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionSearchSpecifications.SearchPermissionsSpec(
            request.SearchTerm,
            request.Category,
            request.IncludeInactive,
            request.Skip,
            request.Take);

        var permissions = await authorizationRepository.GetPermissionsWithSpecification(spec, cancellationToken);
        return Result.Success(mapper.Map<List<PermissionDto>>(permissions));
    }
}