using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.Json;

public class IngresoJsonRepository : IIngresoRepository
{
    private readonly string _ingresoFilePath;
    private readonly string _parqueoFilePath;
    private readonly string _automovilFilePath;

    private static readonly JsonSerializerOptions SerializerOptions =
        new() { WriteIndented = true };

    public IngresoJsonRepository(string jsonDataPath)
    {
        _ingresoFilePath   = Path.Combine(jsonDataPath, "ingresos.json");
        _parqueoFilePath   = Path.Combine(jsonDataPath, "parqueos.json");
        _automovilFilePath = Path.Combine(jsonDataPath, "automoviles.json");
    }

    // ── Helpers ───────────────────────────────────────────────────

    private List<Ingreso> LoadAll()
    {
        if (!File.Exists(_ingresoFilePath))
            return new List<Ingreso>();

        var json = File.ReadAllText(_ingresoFilePath);
        return JsonSerializer.Deserialize<List<Ingreso>>(json)
               ?? new List<Ingreso>();
    }

    private List<Parqueo> LoadParqueos()
    {
        if (!File.Exists(_parqueoFilePath))
            return new List<Parqueo>();

        var json = File.ReadAllText(_parqueoFilePath);
        return JsonSerializer.Deserialize<List<Parqueo>>(json)
               ?? new List<Parqueo>();
    }

    private List<Automovil> LoadAutomoviles()
    {
        if (!File.Exists(_automovilFilePath))
            return new List<Automovil>();

        var json = File.ReadAllText(_automovilFilePath);
        return JsonSerializer.Deserialize<List<Automovil>>(json)
               ?? new List<Automovil>();
    }

    /// <summary>
    /// Loads all ingresos and attaches navigation properties so that
    /// computed fields (DuracionMinutos, DuracionHoras, MontoTotal) work.
    /// </summary>
    private List<Ingreso> LoadAllWithNavigation()
    {
        var ingresos   = LoadAll();
        var parqueos   = LoadParqueos();
        var automoviles = LoadAutomoviles();

        foreach (var ingreso in ingresos)
        {
            ingreso.Parqueo   = parqueos.FirstOrDefault(p => p.Id == ingreso.ParqueoId);
            ingreso.Automovil = automoviles.FirstOrDefault(a => a.Id == ingreso.AutomovilId);
        }

        return ingresos;
    }

    private void SaveAll(List<Ingreso> items)
    {
        // Strip navigation properties before saving — only persist scalar fields.
        var toSave = items.Select(i => new Ingreso
        {
            Consecutivo  = i.Consecutivo,
            ParqueoId    = i.ParqueoId,
            AutomovilId  = i.AutomovilId,
            FechaEntrada = i.FechaEntrada,
            FechaSalida  = i.FechaSalida
        }).ToList();

        var json = JsonSerializer.Serialize(toSave, SerializerOptions);
        File.WriteAllText(_ingresoFilePath, json);
    }

    // ── CRUD ──────────────────────────────────────────────────────

    public IEnumerable<Ingreso> GetAll() => LoadAllWithNavigation();

    public Ingreso? GetById(int consecutivo) =>
        LoadAllWithNavigation().FirstOrDefault(i => i.Consecutivo == consecutivo);

    public void Insert(Ingreso ingreso)
    {
        var items = LoadAll();

        if (items.Any(i => i.Consecutivo == ingreso.Consecutivo))
            throw new InvalidOperationException(
                $"Ya existe un ingreso con Consecutivo = {ingreso.Consecutivo}.");

        items.Add(new Ingreso
        {
            Consecutivo  = ingreso.Consecutivo,
            ParqueoId    = ingreso.ParqueoId,
            AutomovilId  = ingreso.AutomovilId,
            FechaEntrada = ingreso.FechaEntrada,
            FechaSalida  = ingreso.FechaSalida
        });

        SaveAll(items);
    }

    public void Update(Ingreso ingreso)
    {
        var items = LoadAll();
        var index = items.FindIndex(i => i.Consecutivo == ingreso.Consecutivo);

        if (index < 0)
            throw new InvalidOperationException(
                $"No se encontró un ingreso con Consecutivo = {ingreso.Consecutivo}.");

        items[index] = new Ingreso
        {
            Consecutivo  = ingreso.Consecutivo,
            ParqueoId    = ingreso.ParqueoId,
            AutomovilId  = ingreso.AutomovilId,
            FechaEntrada = ingreso.FechaEntrada,
            FechaSalida  = ingreso.FechaSalida
        };

        SaveAll(items);
    }

    public void Delete(int consecutivo)
    {
        var items   = LoadAll();
        var removed = items.RemoveAll(i => i.Consecutivo == consecutivo);
        if (removed > 0) SaveAll(items);
    }

    // ── Advanced queries ──────────────────────────────────────────

    public IEnumerable<Ingreso> GetByTipoAutomovilAndDateRange(
        string tipo, DateTime desde, DateTime hasta) =>
        LoadAllWithNavigation()
            .Where(i => i.Automovil is not null
                     && i.Automovil.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase)
                     && i.FechaEntrada >= desde
                     && i.FechaEntrada <= hasta);

    public IEnumerable<Ingreso> GetByProvinciaAndDateRange(
        string provincia, DateTime desde, DateTime hasta) =>
        LoadAllWithNavigation()
            .Where(i => i.Parqueo is not null
                     && i.Parqueo.Provincia.Contains(provincia, StringComparison.OrdinalIgnoreCase)
                     && i.FechaEntrada >= desde
                     && i.FechaEntrada <= hasta);
}
