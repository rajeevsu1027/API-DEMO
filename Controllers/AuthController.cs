using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_DEMO.Models;
using Microsoft.Extensions.Configuration;

namespace API_DEMO.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login login)
        {
            // Validate user (replace with your own logic)
            if (login.Username == "admin" && login.Password == "password")
            {
                var user = new User
                {
                    Username = "admin",
                    UserEmail = "admin@example.com",
                    UserRole = "Admin"
                };

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Role, user.UserRole)
                };

                // Read key from configuration and ensure it's not null
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    return StatusCode(500, "JWT key is not configured.");
                }

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                );
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "YourAppName",
                    audience: "YourAppNameUsers",
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }
    }
}

