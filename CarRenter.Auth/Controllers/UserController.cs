using CarRenter.Auth.Data;
using CarRenter.Auth.Dtos;
using CarRenter.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRenter.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Bearer + token żeby autoryzacja działała
    public class UserController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public UserController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.titiUsers
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.titiUsers.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            });
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto updatedUser)
        {
            var user = await _context.titiUsers.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return Ok("User updated.");
        }

        [HttpPut("editwithpassword/{id}")]
        //[Authorize]
        public async Task<IActionResult> EditWithPassword(int id, [FromBody] UserEditDto updatedUser)
        {
            var user = await _context.titiUsers.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            var passwordHasher = new PasswordHasher<User>();
            if (!string.IsNullOrWhiteSpace(updatedUser.NewPassword))
            {
                user.PasswordHash = passwordHasher.HashPassword(user, updatedUser.NewPassword); // Możesz użyć własnej metody haszowania
            }

            await _context.SaveChangesAsync();
            return Ok("User updated.");
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.titiUsers.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.titiUsers.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("User deleted.");
        }
    }
}
