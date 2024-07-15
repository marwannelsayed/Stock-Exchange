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

        public AuthController(IConfiguration configuration, User user, StockExchangeDBContext context)
        {
            _configuration = configuration;
            _user = user;
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
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var jwtExpiresInMinutes = Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "test"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtExpiresInMinutes)),
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
