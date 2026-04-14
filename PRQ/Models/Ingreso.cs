using System.Text.Json.Serialization;

namespace PRQ.Models;

public class Ingreso
{
    public int       Consecutivo  { get; set; }
    public int       ParqueoId    { get; set; }
    public int       AutomovilId  { get; set; }
    public DateTime  FechaEntrada { get; set; }
    public DateTime? FechaSalida  { get; set; }

    // ── Navigation properties ───────────────────────────────────────
    [JsonIgnore]
    public Parqueo?   Parqueo   { get; set; }

    [JsonIgnore]
    public Automovil? Automovil { get; set; }

    // ── Computed properties (null when FechaSalida is null) ─────────
    public double? DuracionMinutos =>
        FechaSalida.HasValue
            ? (FechaSalida.Value - FechaEntrada).TotalMinutes
            : null;

    public double? DuracionHoras =>
        FechaSalida.HasValue
            ? (FechaSalida.Value - FechaEntrada).TotalHours
            : null;

    public decimal? MontoTotal =>
        (FechaSalida.HasValue && Parqueo is not null)
            ? (decimal)DuracionHoras!.Value * Parqueo.PrecioPorHora
            : null;
}
