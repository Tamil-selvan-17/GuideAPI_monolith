using AutoMapper;
using CleanMonolith.Application.DTOs;
using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<User, UserDto>()
        //    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            
        //CreateMap<CreateUserDto, User>()
        //    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (Domain.Enums.Role)src.RoleId));
            
        CreateMap<UpdateUserDto, UserMaster>();
    }
}
