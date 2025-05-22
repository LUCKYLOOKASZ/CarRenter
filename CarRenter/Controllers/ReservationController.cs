using CarRenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarRenter.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReservationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int vehicleId)
        {
            //var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

            //var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            

            //int? parsedUserId = null;
            //if (!string.IsNullOrEmpty(currentUserId) && int.TryParse(currentUserId, out int uid))
            //{
            //    parsedUserId = uid;
            //}

            var model = new CreateReservationViewModel
            {
                VehicleId = vehicleId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                CurrentUserId = currentUserId
            };

            // Pobierz istniejące rezerwacje dla pojazdu
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"http://localhost:6000/Reservation/by-vehicle/{vehicleId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                model.ExistingReservations = JsonSerializer.Deserialize<List<ReservationDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
          
            var existingResponse = await client.GetAsync($"http://localhost:6000/Reservation/by-vehicle/{model.VehicleId}");
            var existingReservations = new List<ReservationDto>();
            if (existingResponse.IsSuccessStatusCode)
            {
                var existingContent = await existingResponse.Content.ReadAsStringAsync();
                existingReservations = JsonSerializer.Deserialize<List<ReservationDto>>(existingContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            bool isOverlapping = existingReservations.Any(r =>
                model.StartDate < r.EndDate && model.EndDate > r.StartDate
            );

            if (isOverlapping)
            {
                ModelState.AddModelError("", "Wybrany termin jest już zajęty dla tego pojazdu.");
                model.ExistingReservations = existingReservations;
                return View(model);
            }

            var reservationDto = new
            {
                VehicleId = model.VehicleId,
                UserId = model.CurrentUserId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Notes = model.Notes
            };

            var content = new StringContent(JsonSerializer.Serialize(reservationDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:6000/Reservation", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Details", "Car", new { id = model.VehicleId });

            ModelState.AddModelError("", "Nie udało się utworzyć rezerwacji.");
            model.ExistingReservations = existingReservations;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int vehicleId)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"http://localhost:6000/Reservation/{id}");

            // Po usunięciu wróć na stronę szczegółów auta lub listę rezerwacji
            return RedirectToAction("Details", "Car", new { id = vehicleId });

        }


    }
}
