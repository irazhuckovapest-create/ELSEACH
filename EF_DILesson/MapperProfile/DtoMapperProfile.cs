using AutoMapper;
using EF_DILesson.Dto;
using EF_DILesson.Models;

namespace EF_DILesson.MapperProfile
{
    public class DtoMapperProfile : Profile
    {
        public DtoMapperProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(u => u.UserName, o => o.MapFrom(d => d.Username))
                .ForMember(u => u.Id, o => o.Ignore());

            CreateMap<User, UserDto>()
                .ForMember(d => d.Username, o => o.MapFrom(u => u.UserName));
        }
    }
}