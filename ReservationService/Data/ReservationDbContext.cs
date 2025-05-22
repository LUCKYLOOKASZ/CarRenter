using Microsoft.EntityFrameworkCore;
using ReservationService.Models;

public class ReservationDbContext : DbContext
{
    public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Reservation> Reservations { get; set; }
}
