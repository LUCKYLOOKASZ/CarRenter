using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleService.Data;
using VehicleService.Dtos;
using VehicleService.Models;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly VehicleDbContext _context;

    public VehiclesController(VehicleDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
    {
        return await _context.titiVehicles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id)
    {
        var vehicle = await _context.titiVehicles.FindAsync(id);
        if (vehicle == null) return NotFound();
        return vehicle;
    }

    [HttpPost]
    public async Task<ActionResult<Vehicle>> CreateVehicle(CreateVehicleDto dto)
    {
        var vehicle = new Vehicle
        {
            Marka = dto.Marka,
            Model = dto.Model,
            Rok = dto.Rok,
            KwotaZaDzien = dto.KwotaZaDzien,
            Opis = dto.Opis,
            UrlObrazka = dto.UrlObrazka
        };

        _context.titiVehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, CreateVehicleDto dto)
    {
        var vehicle = await _context.titiVehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        vehicle.Marka = dto.Marka;
        vehicle.Model = dto.Model;
        vehicle.Rok = dto.Rok;
        vehicle.KwotaZaDzien = dto.KwotaZaDzien;
        vehicle.Opis = dto.Opis;
        vehicle.UrlObrazka = dto.UrlObrazka;

        _context.Entry(vehicle).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _context.titiVehicles.FindAsync(id);
        if (vehicle == null) return NotFound();

        _context.titiVehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool VehicleExists(int id) => _context.titiVehicles.Any(e => e.Id == id);
}
