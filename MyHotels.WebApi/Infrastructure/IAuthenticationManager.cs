using MyHotels.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Infrastructure
{
    interface IAuthenticationManager
    {
        Task<bool> ValidateApiUser(LoginApiUserDto userDto);
        Task<string> CreateJwtToken();
    }
}
