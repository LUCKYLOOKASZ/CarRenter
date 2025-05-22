using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationService.Models;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly ReservationDbContext _context;

    public ReservationController(ReservationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
    {
        return await _context.Reservations.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetById(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return NotFound();
        return reservation;
    }

    [HttpGet("by-vehicle/{vehicleId}")]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetByVehicleId(int vehicleId)
    {
        var reservations = await _context.Reservations
            .Where(r => r.VehicleId == vehicleId)
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> Create(Reservation reservation)
    {
        reservation.CreatedAt = DateTime.UtcNow;
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, reservation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Reservation reservation)
    {
        if (id != reservation.ReservationId)
            return BadRequest();

        var existing = await _context.Reservations.FindAsync(id);
        if (existing == null)
            return NotFound();

        // Zaktualizuj właściwości
        existing.StartDate = reservation.StartDate;
        existing.EndDate = reservation.EndDate;
        existing.UserId = reservation.UserId;
        existing.VehicleId = reservation.VehicleId;
        existing.Notes = reservation.Notes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return NotFound();

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
