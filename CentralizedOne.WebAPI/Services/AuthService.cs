using CentralizedOne.Data;
using CentralizedOne.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CentralizedOne.WebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public User? ValidateUser(string username, string password, ApplicationDbContext db)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && User.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
        // Generate JWT Token 
        public string Authenticate(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // 🆕 employee ID
                new Claim(ClaimTypes.Name, user.Username),                 // username
                new Claim(ClaimTypes.Role, user.Role)                      // role
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
