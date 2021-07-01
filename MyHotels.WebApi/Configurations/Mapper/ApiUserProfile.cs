using AutoMapper;
using MyHotels.WebApi.Domain;
using MyHotels.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Configurations.Mapper
{
    public class ApiUserProfile : Profile
    {
        public ApiUserProfile()
        {
            CreateMap<ApiUser, LoginApiUserDto>().ReverseMap();
            CreateMap<ApiUser, RegisterApiUserDto>().ReverseMap();
        }
    }
}
