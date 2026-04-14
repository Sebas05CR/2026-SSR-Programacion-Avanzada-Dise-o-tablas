using System;
using System.Collections.Generic;
using System.Linq;
using PRQ.Data;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.DB;

public class ParqueoDbRepository : IParqueoRepository
{
    private readonly PRQDbContext _context;

    public ParqueoDbRepository(PRQDbContext context)
    {
        _context = context;
    }

    // ── CRUD ──────────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetAll() =>
        _context.Parqueos.ToList();

    public Parqueo? GetById(int id) =>
        _context.Parqueos.Find(id);

    public void Insert(Parqueo parqueo)
    {
        _context.Parqueos.Add(parqueo);
        _context.SaveChanges();
    }

    public void Update(Parqueo parqueo)
    {
        _context.Parqueos.Update(parqueo);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _context.Parqueos.Find(id);
        if (entity is not null)
        {
            _context.Parqueos.Remove(entity);
            _context.SaveChanges();
        }
    }

    // ── Filters ───────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetByProvincia(string provincia) =>
        _context.Parqueos
            .Where(p => p.Provincia.ToLower().Contains(provincia.ToLower()))
            .ToList();

    public IEnumerable<Parqueo> GetByNombre(string nombre) =>
        _context.Parqueos
            .Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()))
            .ToList();

    public IEnumerable<Parqueo> GetByPrecioRange(decimal precioMin, decimal precioMax) =>
        _context.Parqueos
            .Where(p => p.PrecioPorHora >= precioMin && p.PrecioPorHora <= precioMax)
            .ToList();

    // ── Advanced ──────────────────────────────────────────────────

    public decimal ObtenerPrecioPorHoraPorParqueo(int parqueoId)
    {
        var parqueo = _context.Parqueos.Find(parqueoId)
            ?? throw new InvalidOperationException(
                   $"No se encontró un parqueo con Id = {parqueoId}.");
        return parqueo.PrecioPorHora;
    }
}
