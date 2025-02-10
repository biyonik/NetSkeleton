using Application.Common.Results;
using Application.Permissions.Dtos;
using MediatR;

namespace Application.Permissions.Queries;

public record GetPermissionEndpointsQuery(Guid PermissionId) 
    : IRequest<Result<List<PermissionEndpointDto>>>;