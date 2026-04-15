using Microsoft.EntityFrameworkCore;
using PRQ.Data;
using PRQ.Interfaces;
using PRQ.Repositories;
using PRQ.Repositories.DB;
using PRQ.Repositories.Json;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

string defaultSource = Environment.GetEnvironmentVariable("PRQ_SOURCE") ?? "DB";
string connStr = Environment.GetEnvironmentVariable("PRQ_CONN")
    ?? throw new InvalidOperationException(
        "Connection string not found. Set the PRQ_CONN environment variable before running.");

string jsonDir = Path.Combine(AppContext.BaseDirectory, "JsonData");
string automovilesPath = Path.Combine(jsonDir, "automoviles.json");
string parqueosPath = Path.Combine(jsonDir, "parqueos.json");
string ingresosPath = Path.Combine(jsonDir, "ingresos.json");

builder.Services.AddDbContext<PRQDbContext>(options =>
    options.UseSqlServer(connStr));

builder.Services.AddSingleton(new ApiSourceSelectorOptions
{
    DefaultSource = defaultSource,
    HeaderName = ApiSourceSelectorOptions.DefaultHeaderName
});
builder.Services.AddScoped<IApiSourceSelector, ApiSourceSelector>();

builder.Services.AddScoped<AutomovilDbRepository>();
builder.Services.AddScoped<ParqueoDbRepository>();
builder.Services.AddScoped<IngresoDbRepository>();

builder.Services.AddSingleton(_ => new AutomovilJsonRepository(automovilesPath));
builder.Services.AddSingleton(_ => new ParqueoJsonRepository(parqueosPath));
builder.Services.AddSingleton(_ => new IngresoJsonRepository(ingresosPath, automovilesPath, parqueosPath));

builder.Services.AddScoped<IAutomovilRepository, RoutedAutomovilRepository>();
builder.Services.AddScoped<IParqueoRepository, RoutedParqueoRepository>();
builder.Services.AddScoped<IIngresoRepository, RoutedIngresoRepository>();

// ── Pipeline ──────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


