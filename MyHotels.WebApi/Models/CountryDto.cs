using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Models
{
    public class CreateCountryDto
    {
        [Required]
        [StringLength(maximumLength: 32, ErrorMessage = "Country name too long!")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 2, MinimumLength = 2, ErrorMessage = "Country code should consists of two characters!")]
        public string Code { get; set; }

    }

    public class UpdateCountryDto : CreateCountryDto
    {
        public virtual IList<HotelDto> Hotels { get; set; }
    }

    public class CountryDto : CreateCountryDto
    {
        [Required]
        public int Id { get; set; }
        public virtual IList<HotelDto> Hotels { get; set; }
    }
}
