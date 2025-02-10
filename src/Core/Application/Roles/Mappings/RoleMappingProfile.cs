using Application.Roles.Dtos;
using AutoMapper;
using Domain.Identity.Models;

namespace Application.Roles.Mappings;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<ApplicationRole, RoleDto>()
            .ForMember(d => d.Permissions, opt => opt.Ignore());
    }
}