using Application.Permissions.Dtos;
using AutoMapper;
using Domain.Authorization;

namespace Application.Permissions.Mappings;

public class PermissionMappingProfile : Profile
{
    public PermissionMappingProfile()
    {
        CreateMap<Permission, PermissionDto>();
    }
}
