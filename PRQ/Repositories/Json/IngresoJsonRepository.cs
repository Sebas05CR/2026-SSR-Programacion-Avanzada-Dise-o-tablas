using System.Text.Json;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.Json;

public class IngresoJsonRepository : IIngresoRepository
{
    private readonly string _ingresosPath;
    private readonly string _automovilesPath;
    private readonly string _parqueosPath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public IngresoJsonRepository(
        string ingresosPath,
        string automovilesPath,
        string parqueosPath)
    {
        _ingresosPath    = ingresosPath;
        _automovilesPath = automovilesPath;
        _parqueosPath    = parqueosPath;
    }

    // ── Persistence helpers ───────────────────────────────────────────────

    private List<Ingreso> Load()
    {
        if (!File.Exists(_ingresosPath))
            return new List<Ingreso>();

        var json = File.ReadAllText(_ingresosPath);
        return JsonSerializer.Deserialize<List<Ingreso>>(json, _jsonOptions)
               ?? new List<Ingreso>();
    }

    private void Save(List<Ingreso> ingresos)
        => File.WriteAllText(_ingresosPath,
               JsonSerializer.Serialize(ingresos, _jsonOptions));

    private List<Automovil> LoadAutomoviles()
    {
        if (!File.Exists(_automovilesPath))
            return new List<Automovil>();

        var json = File.ReadAllText(_automovilesPath);
        return JsonSerializer.Deserialize<List<Automovil>>(json, _jsonOptions)
               ?? new List<Automovil>();
    }

    private List<Parqueo> LoadParqueos()
    {
        if (!File.Exists(_parqueosPath))
            return new List<Parqueo>();

        var json = File.ReadAllText(_parqueosPath);
        return JsonSerializer.Deserialize<List<Parqueo>>(json, _jsonOptions)
               ?? new List<Parqueo>();
    }

    // Populate Automovil and Parqueo navigation properties via in-memory join.
    private List<Ingreso> LoadWithNavigation()
    {
        var ingresos   = Load();
        var automoviles = LoadAutomoviles();
        var parqueos    = LoadParqueos();

        foreach (var ingreso in ingresos)
        {
            ingreso.Automovil = automoviles.FirstOrDefault(a => a.Id == ingreso.AutomovilId);
            ingreso.Parqueo   = parqueos.FirstOrDefault(p => p.Id == ingreso.ParqueoId);
        }

        return ingresos;
    }

    // ── CRUD ──────────────────────────────────────────────────────────────

    public IEnumerable<Ingreso> GetAll()
        => LoadWithNavigation();

    public Ingreso? GetById(int id)
        => LoadWithNavigation().FirstOrDefault(i => i.Consecutivo == id);

    public void Insert(Ingreso ingreso)
    {
        var list = Load();
        ingreso.Consecutivo = list.Count > 0 ? list.Max(i => i.Consecutivo) + 1 : 1;
        list.Add(ingreso);
        Save(list);
    }

    public void Update(Ingreso ingreso)
    {
        var list  = Load();
        var index = list.FindIndex(i => i.Consecutivo == ingreso.Consecutivo);
        if (index >= 0)
        {
            list[index] = ingreso;
            Save(list);
        }
    }

    public void Delete(int id)
    {
        var list = Load();
        var item = list.FirstOrDefault(i => i.Consecutivo == id);
        if (item is not null)
        {
            list.Remove(item);
            Save(list);
        }
    }

    // ── Queries ───────────────────────────────────────────────────────────

    public IEnumerable<Ingreso> GetByTipoAndDateRange(
        string tipo, DateTime start, DateTime end)
        => LoadWithNavigation().Where(i =>
            i.Automovil is not null &&
            i.Automovil.Tipo.Contains(tipo, StringComparison.OrdinalIgnoreCase) &&
            i.FechaEntrada >= start &&
            i.FechaEntrada <= end);

    public IEnumerable<Ingreso> GetByProvinciaAndDateRange(
        string provincia, DateTime start, DateTime end)
        => LoadWithNavigation().Where(i =>
            i.Parqueo is not null &&
            i.Parqueo.Provincia.Contains(provincia, StringComparison.OrdinalIgnoreCase) &&
            i.FechaEntrada >= start &&
            i.FechaEntrada <= end);
}
