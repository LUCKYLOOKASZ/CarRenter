using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using CarRenter.Models;

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
        var response = await client.GetAsync($"http://localhost:6000/vehicles/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var content = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<VehicleViewModel>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return View(vehicle);
    }

}
