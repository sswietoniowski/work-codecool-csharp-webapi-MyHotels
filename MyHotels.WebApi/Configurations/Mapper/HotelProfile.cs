using AutoMapper;
using MyHotels.WebApi.Domain;
using MyHotels.WebApi.Models;

namespace MyHotels.WebApi.Configurations.Mapper;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<Hotel, HotelDto>().ReverseMap();
        CreateMap<Hotel, CreateHotelDto>().ReverseMap();
        CreateMap<Hotel, UpdateHotelDto>().ReverseMap();
    }
}