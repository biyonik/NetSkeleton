using Application.Common.Results;
using Application.Permissions.Queries;
using Domain.Authorization.Repositories;
using MediatR;

namespace Application.Permissions.Handlers;

public class GetPermissionCategoriesQueryHandler(IAuthorizationRepository authorizationRepository)
    : IRequestHandler<GetPermissionCategoriesQuery, Result<List<string>>>
{
    public async Task<Result<List<string>>> Handle(GetPermissionCategoriesQuery request, CancellationToken cancellationToken)
    {
        var permissions = await authorizationRepository.GetAllPermissionsAsync(false, cancellationToken);
        var categories = permissions
            .Where(p => !string.IsNullOrEmpty(p.Category))
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        return Result.Success(categories);
    }
}