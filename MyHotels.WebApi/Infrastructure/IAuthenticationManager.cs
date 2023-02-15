using MyHotels.WebApi.Models;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Infrastructure;

public interface IAuthenticationManager
{
    Task<bool> ValidateApiUser(LoginApiUserDto userDto);
    Task<string> CreateJwtToken();
}