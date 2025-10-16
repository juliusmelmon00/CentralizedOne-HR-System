using CentralizedOne.Data;
using CentralizedOne.Data.Models;
using CentralizedOne.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuthService _authService;

        public AuthController(ApplicationDbContext db, IAuthService authService)
        {
            _db = db;
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _authService.ValidateUser(request.Username, request.Password, _db);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var token = _authService.Authenticate(user);

            return Ok(new
            {
                token,
                username = user.Username,
                role = user.Role
            });
        }
    }

    // DTO for login request
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
