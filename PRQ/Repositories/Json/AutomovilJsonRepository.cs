using System.Text.Json;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.Json;

public class AutomovilJsonRepository : IAutomovilRepository
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public AutomovilJsonRepository(string filePath)
    {
        _filePath = filePath;
    }

    // ── Persistence helpers ───────────────────────────────────────────────

    private List<Automovil> Load()
    {
        if (!File.Exists(_filePath))
            return new List<Automovil>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Automovil>>(json, _jsonOptions)
               ?? new List<Automovil>();
    }

    private void Save(List<Automovil> automoviles)
        => File.WriteAllText(_filePath, JsonSerializer.Serialize(automoviles, _jsonOptions));

    // ── CRUD ──────────────────────────────────────────────────────────────

    public IEnumerable<Automovil> GetAll()
        => Load();

    public Automovil? GetById(int id)
        => Load().FirstOrDefault(a => a.Id == id);

    public void Insert(Automovil automovil)
    {
        var list = Load();
        automovil.Id = list.Count > 0 ? list.Max(a => a.Id) + 1 : 1;
        list.Add(automovil);
        Save(list);
    }

    public void Update(Automovil automovil)
    {
        var list  = Load();
        var index = list.FindIndex(a => a.Id == automovil.Id);
        if (index >= 0)
        {
            list[index] = automovil;
            Save(list);
        }
    }

    public void Delete(int id)
    {
        var list = Load();
        var item = list.FirstOrDefault(a => a.Id == id);
        if (item is not null)
        {
            list.Remove(item);
            Save(list);
        }
    }

    // ── Filters ───────────────────────────────────────────────────────────

    public IEnumerable<Automovil> GetByColor(string color)
        => Load().Where(a => a.Color.Contains(color, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Automovil> GetByYearRange(int startYear, int endYear)
        => Load().Where(a => a.Anio >= startYear && a.Anio <= endYear);

    public IEnumerable<Automovil> GetByFabricante(string fabricante)
        => Load().Where(a => a.Fabricante.Contains(fabricante, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Automovil> GetByTipo(string tipo)
        => Load().Where(a => a.Tipo.Contains(tipo, StringComparison.OrdinalIgnoreCase));
}
