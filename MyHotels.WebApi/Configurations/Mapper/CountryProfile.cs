using AutoMapper;
using MyHotels.WebApi.Domain;
using MyHotels.WebApi.Models;

namespace MyHotels.WebApi.Configurations.Mapper;

public class CountryProfile : Profile
{
    public CountryProfile()
    {
        CreateMap<Country, CreateCountryDto>().ReverseMap();
        CreateMap<Country, UpdateCountryDto>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();
    }
}