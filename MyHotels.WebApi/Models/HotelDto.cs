using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Models
{
    public class CreateHotelDto
    {
        [Required]
        [StringLength(maximumLength: 64, ErrorMessage = "Hotel name is too long!")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 64, ErrorMessage = "Hotel address is too long!")]
        public string Address { get; set; }
        [Required]
        [Range(1.0, 6.0)]
        public double Rating { get; set; }
        [Required]
        public int CountryId { get; set; }
    }

    public class UpdateHotelDto : CreateHotelDto
    {
        public CountryDto Country { get; set; }
    }

    public class HotelDto : CreateHotelDto
    {
        [Required]
        public int Id { get; set; }
        public CountryDto Country { get; set; }
    }
}
