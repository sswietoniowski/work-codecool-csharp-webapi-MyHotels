using Microsoft.EntityFrameworkCore;
using MyHotels.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                    new Hotel { Id = 1, CountryId = 1, Name = "Hotel Piast", Address = "Wrocław", Rating = 4.0 },
                    new Hotel { Id = 2, CountryId = 1, Name = "Center Warsaw", Address = "Warszawa", Rating = 4.5 },
                    new Hotel { Id = 3, CountryId = 2, Name = "Waldorf Astoria", Address = "Berlin", Rating = 5.0 }
                );
        }
    }
}
