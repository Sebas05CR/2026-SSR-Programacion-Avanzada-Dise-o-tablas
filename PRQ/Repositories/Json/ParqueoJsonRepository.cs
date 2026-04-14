using System.Text.Json;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.Json;

public class ParqueoJsonRepository : IParqueoRepository
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public ParqueoJsonRepository(string filePath)
    {
        _filePath = filePath;
    }

    // ── Persistence helpers ───────────────────────────────────────────────

    private List<Parqueo> Load()
    {
        if (!File.Exists(_filePath))
            return new List<Parqueo>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Parqueo>>(json, _jsonOptions)
               ?? new List<Parqueo>();
    }

    private void Save(List<Parqueo> parqueos)
        => File.WriteAllText(_filePath, JsonSerializer.Serialize(parqueos, _jsonOptions));

    // ── CRUD ──────────────────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetAll()
        => Load();

    public Parqueo? GetById(int id)
        => Load().FirstOrDefault(p => p.Id == id);

    public void Insert(Parqueo parqueo)
    {
        var list = Load();
        parqueo.Id = list.Count > 0 ? list.Max(p => p.Id) + 1 : 1;
        list.Add(parqueo);
        Save(list);
    }

    public void Update(Parqueo parqueo)
    {
        var list  = Load();
        var index = list.FindIndex(p => p.Id == parqueo.Id);
        if (index >= 0)
        {
            list[index] = parqueo;
            Save(list);
        }
    }

    public void Delete(int id)
    {
        var list = Load();
        var item = list.FirstOrDefault(p => p.Id == id);
        if (item is not null)
        {
            list.Remove(item);
            Save(list);
        }
    }

    // ── Filters ───────────────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetByProvincia(string provincia)
        => Load().Where(p => p.Provincia.Contains(provincia, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Parqueo> GetByNombre(string nombre)
        => Load().Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Parqueo> GetByPriceRange(decimal minPrice, decimal maxPrice)
        => Load().Where(p => p.PrecioPorHora >= minPrice && p.PrecioPorHora <= maxPrice);

    public decimal ObtenerPrecioPorHoraPorParqueo(int parqueoId)
        => Load().FirstOrDefault(p => p.Id == parqueoId)?.PrecioPorHora ?? 0m;
}
