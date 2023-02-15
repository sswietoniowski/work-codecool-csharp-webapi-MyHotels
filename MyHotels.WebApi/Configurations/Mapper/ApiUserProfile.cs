using AutoMapper;
using MyHotels.WebApi.Domain;
using MyHotels.WebApi.Models;

namespace MyHotels.WebApi.Configurations.Mapper;

public class ApiUserProfile : Profile
{
    public ApiUserProfile()
    {
        CreateMap<ApiUser, LoginApiUserDto>().ReverseMap();
        CreateMap<ApiUser, RegisterApiUserDto>().ReverseMap();
    }
}