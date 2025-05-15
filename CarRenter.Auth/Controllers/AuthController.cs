using CarRenter.Auth.Data;
using CarRenter.Auth.Dtos;
using CarRenter.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRenter.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AuthDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.titiUsers.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists.");

            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = "User"
            };
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

            try
            {
                _context.titiUsers.Add(user);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message;
                var detailedMessage = ex.InnerException?.ToString();
                Console.WriteLine("Error occurred while saving changes: " + detailedMessage);
                return StatusCode(500, "An error occurred while saving data: " + innerExceptionMessage);
            }

            return Ok("User successfully registered.");
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] Dtos.LoginRequest dto)
        {
            var user = _context.titiUsers.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null)
                return Unauthorized("User not found");

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid password");

            // Token generowany na podstawie danych użytkownika
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }




    }
}
