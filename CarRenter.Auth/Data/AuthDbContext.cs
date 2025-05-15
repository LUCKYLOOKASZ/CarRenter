using CarRenter.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRenter.Auth.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
