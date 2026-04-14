using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.Json;

public class ParqueoJsonRepository : IParqueoRepository
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions SerializerOptions =
        new() { WriteIndented = true };

    public ParqueoJsonRepository(string jsonDataPath)
    {
        _filePath = Path.Combine(jsonDataPath, "parqueos.json");
    }

    // ── Helpers ───────────────────────────────────────────────────

    private List<Parqueo> LoadAll()
    {
        if (!File.Exists(_filePath))
            return new List<Parqueo>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Parqueo>>(json)
               ?? new List<Parqueo>();
    }

    private void SaveAll(List<Parqueo> items)
    {
        var json = JsonSerializer.Serialize(items, SerializerOptions);
        File.WriteAllText(_filePath, json);
    }

    // ── CRUD ──────────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetAll() => LoadAll();

    public Parqueo? GetById(int id) =>
        LoadAll().FirstOrDefault(p => p.Id == id);

    public void Insert(Parqueo parqueo)
    {
        var items = LoadAll();

        if (items.Any(p => p.Id == parqueo.Id))
            throw new InvalidOperationException(
                $"Ya existe un parqueo con Id = {parqueo.Id}.");

        items.Add(parqueo);
        SaveAll(items);
    }

    public void Update(Parqueo parqueo)
    {
        var items = LoadAll();
        var index = items.FindIndex(p => p.Id == parqueo.Id);

        if (index < 0)
            throw new InvalidOperationException(
                $"No se encontró un parqueo con Id = {parqueo.Id}.");

        items[index] = parqueo;
        SaveAll(items);
    }

    public void Delete(int id)
    {
        var items = LoadAll();
        var removed = items.RemoveAll(p => p.Id == id);
        if (removed > 0) SaveAll(items);
    }

    // ── Filters ───────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetByProvincia(string provincia) =>
        LoadAll()
            .Where(p => p.Provincia.Contains(provincia, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Parqueo> GetByNombre(string nombre) =>
        LoadAll()
            .Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Parqueo> GetByPrecioRange(decimal precioMin, decimal precioMax) =>
        LoadAll()
            .Where(p => p.PrecioPorHora >= precioMin && p.PrecioPorHora <= precioMax);

    // ── Advanced ──────────────────────────────────────────────────

    public decimal ObtenerPrecioPorHoraPorParqueo(int parqueoId)
    {
        var parqueo = LoadAll().FirstOrDefault(p => p.Id == parqueoId)
            ?? throw new InvalidOperationException(
                   $"No se encontró un parqueo con Id = {parqueoId}.");
        return parqueo.PrecioPorHora;
    }
}
