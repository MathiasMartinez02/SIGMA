using AutoMapper;
using SIGMA.Application.Users.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName));
    }
}
