using Application.Permissions.Dtos;
using AutoMapper;
using Domain.Authorization;

namespace Application.Permissions.Mappings;

public class PermissionEndpointMappingProfile : Profile
{
    public PermissionEndpointMappingProfile()
    {
        CreateMap<PermissionEndpoint, PermissionEndpointDto>();
    }
}