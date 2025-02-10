using Application.Users.Dtos;
using AutoMapper;
using Domain.Identity.Models;

namespace Application.Users.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(d => d.Roles, opt => opt.Ignore())
            .ForMember(d => d.Permissions, opt => opt.Ignore());
    }
}