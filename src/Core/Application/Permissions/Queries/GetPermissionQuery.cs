using Application.Common.Results;
using Application.Permissions.Dtos;
using MediatR;

namespace Application.Permissions.Queries;

public record GetPermissionQuery(Guid Id) : IRequest<Result<PermissionDto>>;
