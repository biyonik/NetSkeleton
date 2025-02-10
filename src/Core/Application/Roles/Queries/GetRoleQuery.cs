using Application.Common.Results;
using Application.Roles.Dtos;
using MediatR;

namespace Application.Roles.Queries;

public record GetRoleQuery(Guid Id) : IRequest<Result<RoleDto>>;
