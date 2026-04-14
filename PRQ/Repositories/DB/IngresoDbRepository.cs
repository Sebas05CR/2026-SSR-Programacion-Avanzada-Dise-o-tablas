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

    // ── CRUD ──────────────────────────────────────────────────────────────

    public IEnumerable<Ingreso> GetAll()
        => _context.Ingresos
            .AsNoTracking()
            .Include(i => i.Parqueo)
            .Include(i => i.Automovil)
            .ToList();

    public Ingreso? GetById(int id)
        => _context.Ingresos
            .AsNoTracking()
            .Include(i => i.Parqueo)
            .Include(i => i.Automovil)
            .FirstOrDefault(i => i.Consecutivo == id);

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

    public void Delete(int id)
    {
        var entity = _context.Ingresos.Find(id);
        if (entity is not null)
        {
            _context.Ingresos.Remove(entity);
            _context.SaveChanges();
        }
    }

    // ── Queries ───────────────────────────────────────────────────────────

    public IEnumerable<Ingreso> GetByTipoAndDateRange(
        string tipo, DateTime start, DateTime end)
        => _context.Ingresos
            .AsNoTracking()
            .Include(i => i.Parqueo)
            .Include(i => i.Automovil)
            .Where(i =>
                EF.Functions.Like(i.Automovil!.Tipo, $"%{tipo}%") &&
                i.FechaEntrada >= start &&
                i.FechaEntrada <= end)
            .ToList();

    public IEnumerable<Ingreso> GetByProvinciaAndDateRange(
        string provincia, DateTime start, DateTime end)
        => _context.Ingresos
            .AsNoTracking()
            .Include(i => i.Parqueo)
            .Include(i => i.Automovil)
            .Where(i =>
                EF.Functions.Like(i.Parqueo!.Provincia, $"%{provincia}%") &&
                i.FechaEntrada >= start &&
                i.FechaEntrada <= end)
            .ToList();
}
