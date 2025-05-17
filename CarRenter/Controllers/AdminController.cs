using CarRenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarRenter.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            string currentUserRole = null;
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var roleClaim = jwt.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (roleClaim != null)
                    currentUserRole = roleClaim.Value;
            }
            if (currentUserRole != "Admin")
                return Unauthorized();

            var user = HttpContext.User;
            var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            Console.WriteLine($"[DEBUG] IsAuthenticated: {isAuthenticated}");
            Console.WriteLine($"[DEBUG] Roles: {string.Join(",", roles)}");

            //var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("http://localhost:6000/User");
            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserViewModel>>(json);

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.DeleteAsync($"http://localhost:6000/user/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("index");

            TempData["Error"] = "Nie udało się usunąć użytkownika.";
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            string currentUserRole = null;
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var roleClaim = jwt.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (roleClaim != null)
                    currentUserRole = roleClaim.Value;
            }
            if (currentUserRole != "Admin")
                return Unauthorized();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.GetAsync($"http://localhost:6000/user/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("index");

            var content = await response.Content.ReadAsStringAsync();
            var user = System.Text.Json.JsonSerializer.Deserialize<UserViewModel>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"http://localhost:6000/user/{model.Id}", content);

            var responseBody = await response.Content.ReadAsStringAsync(); // DEBUG
            Console.WriteLine($"[DEBUG] PUT Response: {response.StatusCode}, Body: {responseBody}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("index");

            TempData["Error"] = "Nie udało się zaktualizować użytkownika.";
            return View(model);
        }

        
        public async Task<IActionResult> Vehicles()
        {
            var token = HttpContext.Session.GetString("JWToken");
            string currentUserRole = null;
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var roleClaim = jwt.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (roleClaim != null)
                    currentUserRole = roleClaim.Value;
            }
            if (currentUserRole != "Admin")
                return Unauthorized();

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("http://localhost:6000/vehicles");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var vehicles = JsonConvert.DeserializeObject<List<VehicleViewModel>>(json);

            return View(vehicles);
        }

        // GET: Admin/CreateVehicle
        public IActionResult CreateVehicle()
        {
            return View();
        }

        // POST: Admin/CreateVehicle
        [HttpPost]
        public async Task<IActionResult> CreateVehicle(VehicleDto model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            string currentUserRole = null;
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var roleClaim = jwt.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (roleClaim != null)
                    currentUserRole = roleClaim.Value;
            }
            if (currentUserRole != "Admin")
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:6000/Vehicles", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Vehicles");

            ModelState.AddModelError("", "Błąd przy dodawaniu pojazdu");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicle(int id)
        {

            var token = HttpContext.Session.GetString("JWToken");
            string currentUserRole = null;
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var roleClaim = jwt.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (roleClaim != null)
                    currentUserRole = roleClaim.Value;
            }
            if (currentUserRole != "Admin")
                return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"http://localhost:6000/Vehicles/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Nie znaleziono pojazdu lub błąd API.";
                return RedirectToAction("Vehicles");
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Brak danych pojazdu.";
                return RedirectToAction("Vehicles");
            }

            var vehicle = System.Text.Json.JsonSerializer.Deserialize<NowyModelDlaEdycjiAuta>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(vehicle);
        }

        //[HttpPost]
        //public async Task<IActionResult> EditVehicle(VehicleViewModel model)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

        //    var json = System.Text.Json.JsonSerializer.Serialize(model);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await client.PutAsync($"http://localhost:6000/Vehicles/{model.Id}", content);



        //    var responseBody = await response.Content.ReadAsStringAsync(); // DEBUG
        //    Console.WriteLine($"[DEBUG] PUT Response: {response.StatusCode}, Body: {responseBody}");
        //    if (response.IsSuccessStatusCode)
        //        return RedirectToAction("http://localhost:6000/Admin/Vehicles");

        //    var errorContent = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine($"[ERROR] Odpowiedź API: {response.StatusCode}, Treść: {errorContent}");
        //    Console.WriteLine($"[ERROR] json: {json}}}");

        //    TempData["Error"] = "Nie udało się zaktualizować użytkownika.";
        //    return View(model);
        //}
        [HttpPost]
        public async Task<IActionResult> EditVehicle(NowyModelDlaEdycjiAuta model)
        {
            Console.WriteLine($"DEBUG: Odebrane dane - {System.Text.Json.JsonSerializer.Serialize(model)}");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Błędy walidacji:");
                foreach (var key in ModelState.Keys)
                {
                    var entry = ModelState[key];
                    foreach (var error in entry.Errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }


            var client = _httpClientFactory.CreateClient();

            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"http://localhost:6000/Vehicles/{model.id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Vehicles");

            return View(model);
        }

        // POST: Admin/DeleteVehicle/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.DeleteAsync($"http://localhost:6000/vehicles/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Vehicles");

            TempData["Error"] = "Nie udało się usunąć pojazdu";
            return RedirectToAction("Vehicles");
        }
    }
}
