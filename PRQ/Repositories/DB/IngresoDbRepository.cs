using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PRQ.Data;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.DB;

public class IngresoDbRepository : IIngresoRepository
{
    private readonly PRQDbContext _context;

    public IngresoDbRepository(PRQDbContext context)
    {
        _context = context;
    }

    // Eager-load navigation properties so computed fields work.
    private IQueryable<Ingreso> WithIncludes() =>
        _context.Ingresos
            .Include(i => i.Parqueo)
            .Include(i => i.Automovil);

    // ── CRUD ──────────────────────────────────────────────────────

    public IEnumerable<Ingreso> GetAll() =>
        WithIncludes().ToList();

    public Ingreso? GetById(int consecutivo) =>
        WithIncludes().FirstOrDefault(i => i.Consecutivo == consecutivo);

    public void Insert(Ingreso ingreso)
    {
        _context.Ingresos.Add(ingreso);
        _context.SaveChanges();
    }

    public void Update(Ingreso ingreso)
    {
        _context.Ingresos.Update(ingreso);
        _context.SaveChanges();
    }

    public void Delete(int consecutivo)
    {
        var entity = _context.Ingresos.Find(consecutivo);
        if (entity is not null)
        {
            _context.Ingresos.Remove(entity);
            _context.SaveChanges();
        }
    }

    // ── Advanced queries ──────────────────────────────────────────

    public IEnumerable<Ingreso> GetByTipoAutomovilAndDateRange(
        string tipo, DateTime desde, DateTime hasta) =>
        WithIncludes()
            .Where(i => i.Automovil != null
                     && i.Automovil.Tipo.ToLower() == tipo.ToLower()
                     && i.FechaEntrada >= desde
                     && i.FechaEntrada <= hasta)
            .ToList();

    public IEnumerable<Ingreso> GetByProvinciaAndDateRange(
        string provincia, DateTime desde, DateTime hasta) =>
        WithIncludes()
            .Where(i => i.Parqueo != null
                     && i.Parqueo.Provincia.ToLower().Contains(provincia.ToLower())
                     && i.FechaEntrada >= desde
                     && i.FechaEntrada <= hasta)
            .ToList();
}
