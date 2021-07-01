using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyHotels.WebApi.Domain;
using MyHotels.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Infrastructure
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;

        public AuthenticationManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._configuration = configuration;
        }

        public async Task<string> CreateJwtToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var issuer = jwtSettings.GetSection("Issuer").Value;
            var audience = jwtSettings.GetSection("Audience").Value;
            var lifetime = Convert.ToDouble(jwtSettings.GetSection("Lifetime").Value);
            var expires = DateTime.Now.AddMinutes(lifetime);

            var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    expires: expires,
                    claims: claims,
                    signingCredentials: signingCredentials
                );

            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, _user.Email)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("MY_HOTELS_KEY");
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> ValidateApiUser(LoginApiUserDto userDto)
        {
            _user = await _userManager.FindByEmailAsync(userDto.Email);

            return (_user != null && await _userManager.CheckPasswordAsync(_user, userDto.Password));
        }
    }
}
