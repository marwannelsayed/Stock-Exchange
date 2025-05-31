using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockExchangeAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace StockExchangeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private User _user;
        private StockExchangeDBContext _context;

        public AuthController(IConfiguration configuration, StockExchangeDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == login.Username);

            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                {
                    _user = user;
                    var token = GenerateJwtToken();
                    return Ok(new { token });
                }
            }

            return Unauthorized();
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] LoginModel signup)
        {
            var userExists = _context.Users.Any(u => u.Username == signup.Username);

            if (userExists)
            {
                return BadRequest("Username already exists");
            }

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = signup.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(signup.Password)

            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok();
        }

        private string GenerateJwtToken()
        {
            // Fetch the JWT key from environment variables
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException(nameof(jwtKey), "JWT_KEY environment variable is not set.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Fetch JWT configuration values from the appsettings.json configuration
            var jwtIssuer = _configuration.GetValue<string>("Jwt:Issuer");
            if (string.IsNullOrEmpty(jwtIssuer))
            {
                throw new ArgumentNullException(nameof(jwtIssuer), "Jwt:Issuer configuration value is not set.");
            }

            var jwtAudience = _configuration.GetValue<string>("Jwt:Audience");
            if (string.IsNullOrEmpty(jwtAudience))
            {
                throw new ArgumentNullException(nameof(jwtAudience), "Jwt:Audience configuration value is not set.");
            }

            var jwtExpiresInMinutes = _configuration.GetValue<int>("Jwt:ExpiresInMinutes");
            if (jwtExpiresInMinutes == 0)
            {
                throw new ArgumentException("Jwt:ExpiresInMinutes configuration value is not set or is invalid.");
            }


            if (_user == null || string.IsNullOrEmpty(_user.Username))
            {
                throw new ArgumentNullException(nameof(_user.Username), "_user or _user.Username is null or empty.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, _user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
