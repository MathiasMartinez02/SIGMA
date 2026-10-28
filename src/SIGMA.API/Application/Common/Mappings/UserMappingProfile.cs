using AutoMapper;
using SIGMA.API.Users.DTOs;
using SIGMA.API.Entities;

namespace SIGMA.API.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName));
    }
}
