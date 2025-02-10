using Application.Common.Results;
using MediatR;

namespace Application.Permissions.Queries;

public record GetPermissionCategoriesQuery : IRequest<Result<List<string>>>;
