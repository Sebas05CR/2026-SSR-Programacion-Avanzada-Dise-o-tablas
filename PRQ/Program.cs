using Microsoft.EntityFrameworkCore;
using PRQ.Data;
using PRQ.Interfaces;
using PRQ.Repositories.DB;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connStr = Environment.GetEnvironmentVariable("PRQ_CONN")
    ?? throw new InvalidOperationException(
        "Connection string not found. Set the PRQ_CONN environment variable before running.");

builder.Services.AddDbContext<PRQDbContext>(options =>
    options.UseSqlServer(connStr));

// ── Repositories ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IAutomovilRepository, AutomovilDbRepository>();

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


