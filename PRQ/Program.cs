using Microsoft.EntityFrameworkCore;
using PRQ.Data;

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


