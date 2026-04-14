using PRQ.Models;

namespace PRQ.Interfaces;

public interface IIngresoRepository
{
    // ── CRUD ──────────────────────────────────────────────────────────────
    IEnumerable<Ingreso> GetAll();
    Ingreso?             GetById(int id);
    void                 Insert(Ingreso ingreso);
    void                 Update(Ingreso ingreso);
    void                 Delete(int id);

    // ── Queries ───────────────────────────────────────────────────────────
    IEnumerable<Ingreso> GetByTipoAndDateRange(string tipo, DateTime start, DateTime end);
    IEnumerable<Ingreso> GetByProvinciaAndDateRange(string provincia, DateTime start, DateTime end);
}
