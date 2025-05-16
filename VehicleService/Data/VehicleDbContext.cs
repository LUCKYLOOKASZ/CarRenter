using Microsoft.EntityFrameworkCore;
using VehicleService.Models;

namespace VehicleService.Data
{
    public class VehicleDbContext : DbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options) { }

        public DbSet<Vehicle> titiVehicles { get; set; }
    }
}
