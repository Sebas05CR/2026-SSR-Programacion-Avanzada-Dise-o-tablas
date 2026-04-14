using PRQ.Models;

namespace PRQ.Interfaces;

public interface IParqueoRepository
{
    // ── CRUD ──────────────────────────────────────────────────────────────
    IEnumerable<Parqueo> GetAll();
    Parqueo?             GetById(int id);
    void                 Insert(Parqueo parqueo);
    void                 Update(Parqueo parqueo);
    void                 Delete(int id);

    // ── Filters ───────────────────────────────────────────────────────────
    IEnumerable<Parqueo> GetByProvincia(string provincia);
    IEnumerable<Parqueo> GetByNombre(string nombre);
    IEnumerable<Parqueo> GetByPriceRange(decimal minPrice, decimal maxPrice);
    decimal              ObtenerPrecioPorHoraPorParqueo(int parqueoId);
}
