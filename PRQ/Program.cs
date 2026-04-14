using Microsoft.EntityFrameworkCore;
using PRQ.Data;
using PRQ.Interfaces;
using PRQ.Repositories.DB;
using PRQ.Repositories.Json;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string source = Environment.GetEnvironmentVariable("PRQ_SOURCE") ?? "DB";

if (source.Equals("JSON", StringComparison.OrdinalIgnoreCase))
{
    // ── JSON repositories ─────────────────────────────────────────────────
    string jsonDir = Path.Combine(AppContext.BaseDirectory, "JsonData");

    string automovilesPath = Path.Combine(jsonDir, "automoviles.json");
    string parqueosPath    = Path.Combine(jsonDir, "parqueos.json");
    string ingresosPath    = Path.Combine(jsonDir, "ingresos.json");

    builder.Services.AddSingleton<IAutomovilRepository>(
        _ => new AutomovilJsonRepository(automovilesPath));

    builder.Services.AddSingleton<IParqueoRepository>(
        _ => new ParqueoJsonRepository(parqueosPath));

    builder.Services.AddSingleton<IIngresoRepository>(
        _ => new IngresoJsonRepository(ingresosPath, automovilesPath, parqueosPath));
}
else
{
    // ── DB repositories (default) ─────────────────────────────────────────
    string connStr = Environment.GetEnvironmentVariable("PRQ_CONN")
        ?? throw new InvalidOperationException(
            "Connection string not found. Set the PRQ_CONN environment variable before running.");

    builder.Services.AddDbContext<PRQDbContext>(options =>
        options.UseSqlServer(connStr));

    builder.Services.AddScoped<IAutomovilRepository, AutomovilDbRepository>();
    builder.Services.AddScoped<IParqueoRepository, ParqueoDbRepository>();
    builder.Services.AddScoped<IIngresoRepository, IngresoDbRepository>();
}

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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


