using CarRenter.Models;
using CarRenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;
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
        var json = System.Text.Json.JsonSerializer.Serialize(model); // Explicitly specify System.Text.Json.JsonSerializer
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:6001/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);
            var token = jsonDoc.RootElement.GetProperty("token").GetString();

            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("Username", model.Username);

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
        client.BaseAddress = new Uri("http://localhost:6001");

        var content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(model),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError(string.Empty, "Rejestracja nie powiodła się.");
        return View(model);
    }
}
