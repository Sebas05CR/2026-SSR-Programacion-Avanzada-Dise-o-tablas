using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.Json;

public class AutomovilJsonRepository : IAutomovilRepository
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions SerializerOptions =
        new() { WriteIndented = true };

    public AutomovilJsonRepository(string jsonDataPath)
    {
        _filePath = Path.Combine(jsonDataPath, "automoviles.json");
    }

    // ── Helpers ───────────────────────────────────────────────────

    private List<Automovil> LoadAll()
    {
        if (!File.Exists(_filePath))
            return new List<Automovil>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Automovil>>(json)
               ?? new List<Automovil>();
    }

    private void SaveAll(List<Automovil> items)
    {
        var json = JsonSerializer.Serialize(items, SerializerOptions);
        File.WriteAllText(_filePath, json);
    }

    // ── CRUD ──────────────────────────────────────────────────────

    public IEnumerable<Automovil> GetAll() => LoadAll();

    public Automovil? GetById(int id) =>
        LoadAll().FirstOrDefault(a => a.Id == id);

    public void Insert(Automovil automovil)
    {
        var items = LoadAll();

        if (items.Any(a => a.Id == automovil.Id))
            throw new InvalidOperationException(
                $"Ya existe un automóvil con Id = {automovil.Id}.");

        items.Add(automovil);
        SaveAll(items);
    }

    public void Update(Automovil automovil)
    {
        var items = LoadAll();
        var index = items.FindIndex(a => a.Id == automovil.Id);

        if (index < 0)
            throw new InvalidOperationException(
                $"No se encontró un automóvil con Id = {automovil.Id}.");

        items[index] = automovil;
        SaveAll(items);
    }

    public void Delete(int id)
    {
        var items = LoadAll();
        var removed = items.RemoveAll(a => a.Id == id);
        if (removed > 0) SaveAll(items);
    }

    // ── Filters ───────────────────────────────────────────────────

    public IEnumerable<Automovil> GetByColor(string color) =>
        LoadAll()
            .Where(a => a.Color.Contains(color, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Automovil> GetByAnioRange(int anioDesde, int anioHasta) =>
        LoadAll()
            .Where(a => a.Anio >= anioDesde && a.Anio <= anioHasta);

    public IEnumerable<Automovil> GetByFabricante(string fabricante) =>
        LoadAll()
            .Where(a => a.Fabricante.Contains(fabricante, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Automovil> GetByTipo(string tipo) =>
        LoadAll()
            .Where(a => a.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));
}
