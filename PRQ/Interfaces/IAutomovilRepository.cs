using System.Collections.Generic;
using PRQ.Models;

namespace PRQ.Interfaces;

public interface IAutomovilRepository
{
    // ── CRUD ──────────────────────────────────────────────────────
    IEnumerable<Automovil> GetAll();
    Automovil?             GetById(int id);
    void                   Insert(Automovil automovil);
    void                   Update(Automovil automovil);
    void                   Delete(int id);

    // ── Filters ───────────────────────────────────────────────────
    IEnumerable<Automovil> GetByColor(string color);
    IEnumerable<Automovil> GetByAnioRange(int anioDesde, int anioHasta);
    IEnumerable<Automovil> GetByFabricante(string fabricante);
    IEnumerable<Automovil> GetByTipo(string tipo);
}
