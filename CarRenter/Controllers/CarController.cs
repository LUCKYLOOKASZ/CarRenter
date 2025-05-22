using CarRenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

public class CarController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CarController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient();
        // Jeśli masz gateway to np. "http://localhost:6000/vehicles"
        var response = await client.GetAsync("http://localhost:6000/vehicles");

        if (!response.IsSuccessStatusCode)
        {
            // Możesz obsłużyć błąd np. pokazać pustą listę lub stronę błędu
            return View(new List<VehicleViewModel>());
        }

        var content = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<VehicleViewModel>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return View(vehicles);
    }
    public async Task<IActionResult> Details(int id)
    {
        var client = _httpClientFactory.CreateClient();

        // Pobierz dane auta
        var response = await client.GetAsync($"http://localhost:6000/vehicles/{id}");
        if (!response.IsSuccessStatusCode)
            return NotFound();

        var token = HttpContext.Session.GetString("JWToken");
        int? currentUserId = null;
        if (!string.IsNullOrEmpty(token))
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            // Najczęściej userId jest w "sub" lub "nameid"
            var userIdClaim = jwt.Claims.FirstOrDefault(c =>
                c.Type == "sub" || c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid))
                currentUserId = uid;
        }

        var content = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<VehicleViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Pobierz rezerwacje dla pojazdu
        var reservations = new List<ReservationDto>();
        var resResponse = await client.GetAsync($"http://localhost:6000/Reservation/by-vehicle/{id}");
        if (resResponse.IsSuccessStatusCode)
        {
            var resContent = await resResponse.Content.ReadAsStringAsync();
            reservations = JsonSerializer.Deserialize<List<ReservationDto>>(resContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Stwórz wspólny model
        var model = new CarDetailsViewModel
        {
            Vehicle = vehicle,
            Reservations = reservations,
            CurrentUserId = currentUserId
        };

        return View(model);
    }

}
