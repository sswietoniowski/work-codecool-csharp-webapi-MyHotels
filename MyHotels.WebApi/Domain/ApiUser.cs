using Microsoft.AspNetCore.Identity;

namespace MyHotels.WebApi.Domain;

public class ApiUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}