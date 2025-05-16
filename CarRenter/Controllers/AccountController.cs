using CarRenter.Controllers;
using CarRenter.Models;
using CarRenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var client = _httpClientFactory.CreateClient();
        var json = System.Text.Json.JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:6000/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);
            var token = jsonDoc.RootElement.GetProperty("token").GetString();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var username = jwt.Claims.FirstOrDefault(c =>
                c.Type == "unique_name" || c.Type == "name" || c.Type == "sub")?.Value;

            // Poprawne pobieranie userId z JWT
            var userId = jwt.Claims.FirstOrDefault(c =>
                c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == "id")?.Value;

            var role = jwt.Claims.FirstOrDefault(c =>
                c.Type == "role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("Username", username ?? model.Username);
            HttpContext.Session.SetString("Role", role ?? string.Empty);
            HttpContext.Session.SetString("UserId", userId ?? string.Empty);


            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            // Jeśli model jest niepoprawny, wróć do widoku z błędami walidacji
            return View(model);
        }

        var client = _httpClientFactory.CreateClient();
        //client.BaseAddress = new Uri("http://localhost:6000");

        var content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(model), // Explicitly using Newtonsoft.Json.JsonConvert
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("http://localhost:6000/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError(string.Empty, "Rejestracja nie powiodła się.");
        return View(model);
    }

    public async Task<IActionResult> EditSelf()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Nie można znaleźć identyfikatora użytkownika.";
            return RedirectToAction("Index", "Home");
        }

        var response = await client.GetAsync($"http://localhost:6000/user/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Nie udało się pobrać danych użytkownika.";
            return RedirectToAction("Index", "Home");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;

        // Obsługa zarówno tablicy, jak i obiektu
        JsonElement userElement = root;
        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            userElement = root[0];
        }
        else if (root.ValueKind != JsonValueKind.Object)
        {
            TempData["Error"] = "Nieprawidłowy format danych użytkownika.";
            return RedirectToAction("Index", "Home");
        }

        var user = new UserViewModel
        {
            Id = userElement.GetProperty("id").GetInt32(),
            UserName = userElement.TryGetProperty("userName", out var userNameProp)
                ? userNameProp.GetString()
                : (userElement.TryGetProperty("username", out var usernameProp) ? usernameProp.GetString() : null) ?? string.Empty,
            Email = userElement.GetProperty("email").GetString() ?? string.Empty,
            Role = userElement.GetProperty("role").GetString() ?? string.Empty
        };

        return View("EditSelf", user);
    }



    [HttpPost]
    public async Task<IActionResult> EditSelf(UserViewModel model, string NewPassword)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

        var dto = new
        {
            username = model.UserName,
            email = model.Email,
            role = HttpContext.Session.GetString("Role"),
            newPassword = string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword
        };

        var json = System.Text.Json.JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"http://localhost:6000/user/editwithpassword/{model.Id}", content);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index", "Home");

        TempData["Error"] = "Nie udało się zaktualizować danych.";
        return View(model);
    }
}
