using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Models
{
    public class LoginApiUserDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Your password is limited to {2} to {1} characters", MinimumLength = 5)]
        public string Password { get; set; }
    }

    public class RegisterApiUserDto : LoginApiUserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
