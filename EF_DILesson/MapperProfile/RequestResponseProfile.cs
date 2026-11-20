using AutoMapper;
using EF_DILesson.Contracts;
using EF_DILesson.Dto;
using EF_DILesson.Models;

namespace EF_DILesson.MapperProfile
{
    public class RequestResponseProfile : Profile
    {
        public RequestResponseProfile()
        {
            CreateMap<CreateMovieRequest, Film>();
            CreateMap<UpdateMovieRequest, Film>();
            CreateMap<Film, MovieResponse>();
            CreateMap<FilmFilterRequest, FilmFilterDto>();

            CreateMap<CreateCommentRequest, MovieComment>();
            CreateMap<UpdateCommentRequest, MovieComment>();
            CreateMap<MovieComment, CommentResponse>();

            CreateMap<IdentityRegisterRequest, UserDto>().ReverseMap();
            CreateMap<IdentityLoginRequest, UserLoginDto>().ReverseMap();
            CreateMap<UserDto, UserResponce>().ReverseMap();
        }
    }
}