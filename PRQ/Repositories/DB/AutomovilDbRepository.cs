using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PRQ.Data;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Repositories.DB;

public class AutomovilDbRepository : IAutomovilRepository
{
    private readonly PRQDbContext _context;

    public AutomovilDbRepository(PRQDbContext context)
    {
        _context = context;
    }

    // ── CRUD ──────────────────────────────────────────────────────

    public IEnumerable<Automovil> GetAll() =>
        _context.Automoviles.ToList();

    public Automovil? GetById(int id) =>
        _context.Automoviles.Find(id);

    public void Insert(Automovil automovil)
    {
        _context.Automoviles.Add(automovil);
        _context.SaveChanges();
    }

    public void Update(Automovil automovil)
    {
        _context.Automoviles.Update(automovil);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _context.Automoviles.Find(id);
        if (entity is not null)
        {
            _context.Automoviles.Remove(entity);
            _context.SaveChanges();
        }
    }

    // ── Filters ───────────────────────────────────────────────────

    public IEnumerable<Automovil> GetByColor(string color) =>
        _context.Automoviles
            .Where(a => EF.Functions.Like(a.Color, $"%{color}%"))
            .ToList();

    public IEnumerable<Automovil> GetByAnioRange(int anioDesde, int anioHasta) =>
        _context.Automoviles
            .Where(a => a.Anio >= anioDesde && a.Anio <= anioHasta)
            .ToList();

    public IEnumerable<Automovil> GetByFabricante(string fabricante) =>
        _context.Automoviles
            .Where(a => EF.Functions.Like(a.Fabricante, $"%{fabricante}%"))
            .ToList();

    public IEnumerable<Automovil> GetByTipo(string tipo) =>
        _context.Automoviles
            .Where(a => EF.Functions.Like(a.Tipo, tipo))
            .ToList();
}
