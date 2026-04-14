using Microsoft.EntityFrameworkCore;
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

    // ── CRUD ──────────────────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetAll()
        => _context.Parqueos.AsNoTracking().ToList();

    public Parqueo? GetById(int id)
        => _context.Parqueos.AsNoTracking().FirstOrDefault(p => p.Id == id);

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

    // ── Filters ───────────────────────────────────────────────────────────

    public IEnumerable<Parqueo> GetByProvincia(string provincia)
        => _context.Parqueos
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Provincia, $"%{provincia}%"))
            .ToList();

    public IEnumerable<Parqueo> GetByNombre(string nombre)
        => _context.Parqueos
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Nombre, $"%{nombre}%"))
            .ToList();

    public IEnumerable<Parqueo> GetByPriceRange(decimal minPrice, decimal maxPrice)
        => _context.Parqueos
            .AsNoTracking()
            .Where(p => p.PrecioPorHora >= minPrice && p.PrecioPorHora <= maxPrice)
            .ToList();

    public decimal ObtenerPrecioPorHoraPorParqueo(int parqueoId)
    {
        var parqueo = _context.Parqueos
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == parqueoId);

        return parqueo?.PrecioPorHora ?? 0m;
    }
}
