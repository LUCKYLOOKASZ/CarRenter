using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Wczytaj konfiguracjê Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Dodaj Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

// Middleware Ocelot
await app.UseOcelot();

app.Run();
